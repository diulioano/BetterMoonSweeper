// Lista tipos, campos, propriedades e métodos de uma DLL .NET em texto, sem carregar a DLL.
// Uso: dotnet run -- <dll> <saida.txt> [filtro-regex-de-nome-de-tipo]
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length < 2)
{
    Console.WriteLine("Uso: ClassDump <dll> <saida.txt> [filtro-regex]");
    return 1;
}

var filter = args.Length > 2 ? new Regex(args[2], RegexOptions.IgnoreCase) : null;
using var fs = File.OpenRead(args[0]);
using var pe = new PEReader(fs);
var md = pe.GetMetadataReader();
var prov = new NameProvider(md);
var sb = new StringBuilder();
int count = 0;

foreach (var th in md.TypeDefinitions)
{
    var td = md.GetTypeDefinition(th);
    var name = prov.FullName(td);
    if (name == "<Module>") continue;
    if (filter != null && !filter.IsMatch(name)) continue;
    count++;

    var baseType = td.BaseType.IsNil ? "" : " : " + prov.Handle(td.BaseType);
    sb.AppendLine($"class {name}{baseType}");

    foreach (var fh in td.GetFields())
    {
        var f = md.GetFieldDefinition(fh);
        var fname = md.GetString(f.Name);
        if (fname.StartsWith("NativeFieldInfoPtr_") || fname.StartsWith("NativeMethodInfoPtr_")) continue;
        var st = (f.Attributes & FieldAttributes.Static) != 0 ? "static " : "";
        sb.AppendLine($"    field  {st}{f.DecodeSignature(prov, null)} {fname}");
    }

    foreach (var ph in td.GetProperties())
    {
        var p = md.GetPropertyDefinition(ph);
        var sig = p.DecodeSignature(prov, null);
        var acc = p.GetAccessors();
        var rw = (acc.Getter.IsNil ? "" : "get;") + (acc.Setter.IsNil ? "" : "set;");
        var anyAcc = acc.Getter.IsNil ? acc.Setter : acc.Getter;
        var pst = !anyAcc.IsNil && (md.GetMethodDefinition(anyAcc).Attributes & MethodAttributes.Static) != 0 ? "static " : "";
        sb.AppendLine($"    prop   {pst}{sig.ReturnType} {md.GetString(p.Name)} {{{rw}}}");
    }

    foreach (var mh in td.GetMethods())
    {
        var m = md.GetMethodDefinition(mh);
        var mname = md.GetString(m.Name);
        if (mname.StartsWith("get_") || mname.StartsWith("set_") || mname == ".cctor") continue;
        var sig = m.DecodeSignature(prov, null);
        var pnames = m.GetParameters().Select(h => md.GetParameter(h)).Where(p => p.SequenceNumber > 0)
            .OrderBy(p => p.SequenceNumber).Select(p => md.GetString(p.Name)).ToList();
        var ps = sig.ParameterTypes.Select((t, i) => i < pnames.Count ? $"{t} {pnames[i]}" : t);
        var st = (m.Attributes & MethodAttributes.Static) != 0 ? "static " : "";
        sb.AppendLine($"    method {st}{sig.ReturnType} {mname}({string.Join(", ", ps)})");
    }
    sb.AppendLine();
}

File.WriteAllText(args[1], sb.ToString());
Console.WriteLine($"{count} tipos escritos em {args[1]}");
return 0;

sealed class NameProvider : ISignatureTypeProvider<string, object>
{
    private readonly MetadataReader _md;
    public NameProvider(MetadataReader md) => _md = md;

    public string FullName(TypeDefinition td)
    {
        var n = _md.GetString(td.Name);
        if (td.GetDeclaringType() is { IsNil: false } decl)
            return FullName(_md.GetTypeDefinition(decl)) + "." + n;
        var ns = _md.GetString(td.Namespace);
        return ns.Length > 0 ? ns + "." + n : n;
    }

    public string Handle(EntityHandle h) => h.Kind switch
    {
        HandleKind.TypeDefinition => GetTypeFromDefinition(_md, (TypeDefinitionHandle)h, 0),
        HandleKind.TypeReference => GetTypeFromReference(_md, (TypeReferenceHandle)h, 0),
        HandleKind.TypeSpecification => _md.GetTypeSpecification((TypeSpecificationHandle)h).DecodeSignature(this, null),
        _ => "?",
    };

    public string GetPrimitiveType(PrimitiveTypeCode t) => t.ToString().ToLowerInvariant();
    public string GetTypeFromDefinition(MetadataReader r, TypeDefinitionHandle h, byte k) => FullName(r.GetTypeDefinition(h));
    public string GetTypeFromReference(MetadataReader r, TypeReferenceHandle h, byte k)
    {
        var tr = r.GetTypeReference(h);
        var ns = r.GetString(tr.Namespace);
        var n = r.GetString(tr.Name);
        // Mostra o namespace quando o tipo não é do próprio jogo nem do System (ex.: Il2CppSystem.Decimal)
        return ns.Length > 0 && ns != "System" && !ns.StartsWith("UnityEngine") ? ns + "." + n : n;
    }
    public string GetTypeFromSpecification(MetadataReader r, object c, TypeSpecificationHandle h, byte k) =>
        r.GetTypeSpecification(h).DecodeSignature(this, c);
    public string GetSZArrayType(string e) => e + "[]";
    public string GetArrayType(string e, ArrayShape s) => e + "[" + new string(',', s.Rank - 1) + "]";
    public string GetByReferenceType(string e) => "ref " + e;
    public string GetPointerType(string e) => e + "*";
    public string GetGenericInstantiation(string g, ImmutableArray<string> a) => g + "<" + string.Join(", ", a) + ">";
    public string GetGenericTypeParameter(object c, int i) => "T" + i;
    public string GetGenericMethodParameter(object c, int i) => "M" + i;
    public string GetFunctionPointerType(MethodSignature<string> s) => "fnptr";
    public string GetModifiedType(string m, string u, bool req) => u;
    public string GetPinnedType(string e) => e;
}
