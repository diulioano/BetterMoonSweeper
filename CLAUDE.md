# BetterMoonSweeper — contexto para o Claude

Mod para o jogo Steam **Moon Sweepers** (DragDrop Games), co-op de até 16 jogadores.
O design acordado, a lista de classes do jogo e as instruções de build ficam no [README.md](README.md). Mantenha o README atualizado quando decisões mudarem.

## Ambiente

- Jogo: `C:\Program Files (x86)\Steam\steamapps\common\Moon Sweepers`
- Unity 6000.6.0f1, **IL2CPP** (`GameAssembly.dll` + `MoonSweepers_Data\il2cpp_data\Metadata\global-metadata.dat`, versão de metadata 108). Não há DLLs Mono para editar.
- Nomes de classes e métodos não são ofuscados (Compactor, Kiln, AddMoney, CrewBuildMoney, BuildUpgradesPage, Dev*).
- Loader: **BepInEx 6.0.0-be.791** (Unity.IL2CPP win-x64, `winhttp.dll` + `BepInEx\` + `dotnet\` na pasta do jogo) + **MoonSweepers BepInEx Patcher V1.0.0** (Nexus mod 9, `BepInEx\patchers\MoonSweepers.ModEngine\`), instalados em 2026-10-27. O Cpp2IL embutido falha na metadata 108 (erro esperado no log); o patcher gera as DLLs de interop a partir do runtime e corrige o Il2CppInterop para a Unity 6000.6. DLLs geradas em `BepInEx\interop`; log em `<jogo>\BepInEx\LogOutput.log` (sucesso: `Chainloader startup complete`). Steam app id: 5335510.
- Histórico: o MelonLoader 0.7.3 carregava o mod mas não executava (`No Support Module Loaded!`, LavaGang/MelonLoader#1159); está guardado no backup em `melonloader\`. Detalhes em `specs/004-migracao-bepinex/research.md`.
- Log da Unity: `%USERPROFILE%\AppData\LocalLow\DragDropGames\MoonSweepers\Player.log`.
- Saves ficam na Steam Cloud (`LocalLow\...\MoonSessions`); backup antes do primeiro teste com o mod em `saves-antes-bepinex\`.
- Backups em `C:\Users\Diuli\Documents\MoonSweepers-backup-2026-10-27` (estado antes de qualquer loader, `melonloader\`, `Mods-antes-do-rename\`, `saves-antes-bepinex\`).

## Projeto

- `BetterMoonSweeper.csproj`, net6.0, referencia `BepInEx\core\*.dll` (BepInEx, Harmony, Il2CppInterop) e `BepInEx\interop\*.dll` (gerados na primeira execução do jogo com o patcher).
- `dotnet build -c Release` copia `BetterMoonSweeper.dll` para `<jogo>\BepInEx\plugins` (só funciona com o jogo fechado ou antes do plugin ser carregado).
- O plugin é um `BasePlugin` do BepInEx que aplica os patches HarmonyX com `PatchAll`; tipos do jogo vêm do namespace `MoonCleaners`. Não usar `AddComponent` (class injection é a parte frágil na 6000.6); para lógica por quadro, Postfix em `MoonWorld.Update`.
- O mod se chamava JonasMod até 2026-10-27; a pasta do repositório e os ids internos dos nós da árvore (`jonasmod.*`) mantêm o nome antigo de propósito.
- Specs, planos e tarefas ficam em `specs/` (Spec Kit); regras em `.specify/memory/constitution.md`.

## Regras

- Comunicação com o usuário em português.
- Pedir autorização antes de baixar qualquer coisa ou alterar arquivos na pasta do jogo; fazer backup antes.
- O mod é para co-op com amigos que também instalam o mod; não fazer nada que afete jogadores sem o mod.
- JONAS 2.0 (modelo 3D maior do Jonas) é fase 2; no código o Jonas se chama `Joonas` (`MoonJoonasRig`, `MoonJoonasMotion`, `MoonJoonasGauge`).
