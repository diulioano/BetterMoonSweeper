# Research: Migração do mod para BepInEx 6

## R1. Por que o BepInEx funciona e o MelonLoader não

- O MelonLoader 0.7.3 gera as DLLs do jogo (com o Cpp2IL `b5ad444` trocado à mão), mas em runtime o
  Il2CppInterop dele não entende o layout de classes da Unity 6000.6: `Class::Init signatures have
  been exhausted` → `No Support Module Loaded!`.
- A comunidade resolveu isso para o BepInEx 6 be.791. Duas soluções equivalentes:
  - **MoonSweepers BepInEx Patcher** (Nexus mod 9, `BepInEx\patchers\MoonSweepers.ModEngine\MoonSweepers.ModEngine.dll`):
    gera as DLLs de interop a partir do runtime em vez do `global-metadata.dat`, corrige o
    Il2CppInterop para o `Il2CppClass` da 6000.6 e repara referências quebradas. Fonte: guia em
    moonsweepers.org/mods.
  - **Build própria do projeto Mahjong** (github.com/itsRevela/Moon-Sweeper-Mahjong, `docs/loader.md`):
    Cpp2IL `b5ad444` fora do processo + patch `il2cppinterop-unity-6000.6.patch`. Verificada pelo autor
    em 2026-10-25: interop, chainloader, plugin, class injection e Harmony funcionam.
- **Patcher instalado (T004)**: "Moon Sweepers BepInEx Patcher" V1.0.0, arquivo do Nexus
  `Moon Sweepers BepInEx Patcher V1.0.0 9 1 2026-10-25T15-40Z LbNgnkSSN.zip`
  (SHA-256 `b495d504e97b6b4a9e0b0d08162cc1c63f3b7aeed3677f88c18006504c56781f`), contendo só
  `BepInEx\patchers\MoonSweepers.ModEngine\MoonSweepers.ModEngine.dll`
  (SHA-256 `b91bb4d4c12870abe79039a04173bbd5077322a8613bf90159a200a8370b1f94`, 72.704 bytes).
  Checagem rápida das strings: referencia BepInEx, Il2CppInterop (runtime e gerador), AsmResolver,
  `GetCurrentProcess`/`ReadProcessMemory`/`GetModuleHandleW` (lê o próprio processo); nenhuma
  referência a rede, `Process.Start` ou registro.
- **Decision**: BepInEx be.791 oficial + patcher do Nexus (escolha do usuário: é a rota que os outros
  mods usam, então os amigos instalam igual).
- **Esperado no log e inofensivo**: `Unsupported metadata version found! We support 23-106, got 108`
  (o Cpp2IL embutido falha, e o patcher assume) e `Class::Init signatures have been exhausted`.
  Sucesso: o log termina com `Chainloader startup complete`.
- **Risco**: o patcher é um binário do Nexus que não auditamos. Registrar nome, versão e SHA-256 do
  arquivo instalado (T004). Plano B: a build aberta do projeto Mahjong.

## R2. Ponto de entrada do plugin

- **Decision**: `BetterMoonSweeperMain : BepInEx.Unity.IL2CPP.BasePlugin` com
  `[BepInPlugin("com.diuli.bettermoonsweeper", "BetterMoonSweeper", Version)]`. Em `Load()`: guarda
  `Log` (`ManualLogSource`) num campo estático, registra "BetterMoonSweeper {Version} carregado" e
  aplica os patches com `new Harmony(Guid).PatchAll(typeof(BetterMoonSweeperMain).Assembly)`.
- **Rationale**: é o padrão de plugin IL2CPP do BepInEx 6; o MelonLoader aplicava os patches
  sozinho, o BepInEx não.

## R3. Chamada por quadro do atalho Ctrl (spec 002)

- O MelonLoader chamava `OnUpdate()`. O BepInEx IL2CPP não tem esse gancho; a forma usual é
  `AddComponent<T>()`, que depende de *class injection*, justamente a parte que a 6000.6 quebrou.
- **Decision**: Postfix em `MoonWorld.Update()` (existe no dump) chamando `GravityBootsHotkey.Tick()`.
- **Rationale**: sem class injection; `MoonWorld` é o objeto do mundo da partida, então o Postfix só
  roda em partida, uma vez por quadro (**hipótese**: há um único `MoonWorld` ativo). A checagem de
  `MoonSession.InGame` em `Tick()` continua.
- **Alternatives considered**: `AddComponent<T>()` (rejeitado pelo risco acima; o projeto Mahjong
  diz que class injection funciona com o patch dele, mas não sabemos se o patcher do Nexus cobre).
- **Hipótese H5**: o mod **não** fica totalmente livre de class injection. `FactoryTree.CriarNo`
  atribui um `System.Func<int,string>` ao campo `effect` (`Il2CppSystem.Func<int,string>`), e essa
  conversão passa pelo `DelegateSupport` do Il2CppInterop, que injeta uma classe. Se falhar,
  `Injetar()` lança exceção. Hoje ele altera `BranchNames`/`Taglines`/`Colors` antes de criar os nós
  e só marca `_injetado` no fim, então cada nova tentativa (a cada quadro, pelo Prefix de
  `StepProduction`) acrescentaria mais uma aba "Fábrica". Mitigação na T013: montar os nós antes de
  mexer nos ramos e desistir após a primeira falha, logando uma vez.

## R4. Nomes de namespace

- O MelonLoader prefixa namespaces do jogo com `Il2Cpp` (`Il2CppMoonCleaners`). O BepInEx pode não
  prefixar (`MoonCleaners`). **Hipótese** até gerar as DLLs.
- **Decision**: depois do primeiro início com o patcher, regenerar `docs/assembly-csharp.txt` com o
  `tools/ClassDump` sobre `BepInEx\interop\Assembly-CSharp.dll` e ajustar os `using` do mod.
  `Il2CppSystem.Decimal` e `UnityEngine.InputSystem` devem continuar iguais (confirmar).

## R5. Projeto

- Referências: `BepInEx\core\BepInEx.Core.dll`, `BepInEx.Unity.IL2CPP.dll`, `0Harmony.dll`,
  `Il2CppInterop.Runtime.dll`, `Il2CppInterop.Common.dll` e `BepInEx\interop\*.dll`, todas com
  `Private=false`. Alvo continua net6.0 (o BepInEx IL2CPP roda em .NET 6).
- Deploy: `BepInEx\plugins\BetterMoonSweeper.dll`.
- Log: `Melon<...>.Logger.Msg/Error` → `BetterMoonSweeperMain.Log.LogInfo/LogError`.

## R6. Instalação e desinstalação para jogadores

- Instalar: extrair o zip do be.791 na pasta do jogo; colocar o patcher em `BepInEx\patchers\`;
  abrir o jogo uma vez (primeiro início mais lento); colocar `BetterMoonSweeper.dll` em
  `BepInEx\plugins`.
- Desinstalar: apagar `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt`,
  `dotnet\` e `BepInEx\` da pasta do jogo.
- Instalação antiga: apagar `version.dll`, `MelonLoader\`, `Mods\` (com a `JonasMod.dll`),
  `Plugins\`, `UserData\` e `UserLibs\`.

- O guia fixa a versão e o SHA-256 do patcher registrados na T004, para todos usarem o mesmo
  (constituição I).

## R7. Versão

- Muda o loader para todos: `0.3.0` (constituição I).
