# Research: Ligar/desligar a bota de gravidade com Ctrl

Fonte de todos os símbolos: `docs/assembly-csharp.txt` (constituição, princípio V). Como o código do
jogo é IL2CPP, só temos assinaturas; tudo marcado **Hipótese** precisa ser confirmado em jogo.

## R1. Como alternar a bota

- **Decision**: chamar `MoonWalker.ToggleGravityBoots()` no `MoonWalker` local.
- **Rationale**: é o método do próprio jogo. `MoonWalker` também expõe `GravityBootsOn`,
  `OwnsGravityBoots`, `SetGravityBoots(bool)` e o evento `GravityBootsChanged`. Os sons
  `MoonSound.GravityBootsOn/Off` e o estado remoto `MoonRemotePlayer.GravityBootsOn` indicam que o
  fluxo nativo já cuida de som e sincronização (FR-004).
- **Hipótese**: `ToggleGravityBoots()` toca o som e propaga o estado para os outros jogadores, e
  retorna `false` sem fazer nada quando o jogador não tem a bota.
- **Fallback**: se não tocar o som, chamar `SetGravityBoots(!GravityBootsOn)`; se ainda assim
  faltar som, tocar `MoonSound.GravityBootsOn/Off`. Se não sincronizar, investigar o caminho
  usado pela tecla nativa antes de criar qualquer mensagem de rede própria.
- **Alternatives considered**:
  - *Adicionar Ctrl às teclas de `MoonInput.GravityBoots`* (`Binding.keys` é um array de `Key` com
    setter). Seria a solução mais nativa, mas herda o comportamento da tecla original, que pode ser
    "segurar" e não "alternar" (`MoonInput.BootsPadHold` sugere segurar no controle). O pedido é
    alternar, então isso fica como plano B, só se o teste mostrar que a tecla nativa já alterna.
  - *Mexer na gravidade direto* (`Gravity`, `EarthGravity`): fora de cogitação, porque pula som,
    sincronização e as regras do jogo.

## R2. Como achar o jogador local

- **Decision**: `MoonRunOver.LocalWalker` (estático).
- **Rationale**: é o único acessor estático que devolve um `MoonWalker` local; ele usa os campos
  `local` e `nextLocalSearch`, então faz a busca e guarda o resultado sozinho.
- **Hipótese**: ele funciona mesmo sem nenhum `MoonRunOver` na cena.
- **Fallback**: Postfix em `MoonWalker.Update` guardando a instância cujo `UpgradePlayer` é igual a
  `MoonWorld.LocalPlayer`.
- **Alternatives considered**: `Object.FindObjectsOfType<MoonWalker>()` a cada quadro, descartado
  pelo custo e porque não diz qual é o local.

## R3. Como ler o Ctrl

- **Decision**: `MoonInput.PlayKeyboard` (um `Keyboard` do Input System), testando
  `leftCtrlKey.wasPressedThisFrame || rightCtrlKey.wasPressedThisFrame`. Se `PlayKeyboard` for nulo,
  usar `Keyboard.current`.
- **Rationale**: `wasPressedThisFrame` só é verdadeiro no quadro do aperto, então segurar a tecla
  não repete a troca (FR-002). `PlayKeyboard` é o teclado que o jogo usa para jogar.
- **Alternatives considered**: `UnityEngine.Input.GetKeyDown`, descartado porque o jogo usa o Input
  System novo e o input antigo pode estar desligado no build.

## R4. Quando ignorar o atalho (FR-005)

| Situação | Símbolo |
|---|---|
| Fora de uma partida | `!MoonSession.InGame` |
| Digitando (chat) | `MoonInput.Typing` |
| Menu aberto | `walker.MenuBlocked` ou `walker.UIBlocked` (**hipótese**: o jogo os usa para bloquear o input do jogador com UI aberta). Reforço: `MoonFramePacing.MenuOpen()` (**hipótese**) |
| Veículo / robô | `walker.Mounted`, `walker.Passenger` (bool) ou `walker.Riding != null` (`Riding` é `MoonRobotVacuum`) |
| Outros estados ocupados | `walker.Climbing`, `walker.Resting`, `walker.Travelling`, `walker.Held`, `walker.Flying` |
| Construção | `walker.Building` ou `MoonInput.BuildToolInHand` |
| Caído | `walker.Knocked` |
| Sem a bota | `!walker.OwnsGravityBoots` (fallback se não refletir a compra: `MoonWorld.HasGravityBoots(walker.UpgradePlayer)`) |

- **Rejeitado**: `Cursor.lockState != Locked` como sinal de menu. Se o jogo destravar o cursor fora
  de menus (mapa, terceira pessoa), o atalho ficaria sempre bloqueado. Só entra se os testes
  mostrarem que `MenuBlocked`/`UIBlocked`/`MenuOpen()` não cobrem algum menu.

## R5. Onde rodar a checagem

- **Decision**: no `OnUpdate()` do `BetterMoonSweeperMain` (MelonLoader chama uma vez por quadro), delegando
  para uma classe própria `GravityBootsHotkey`.
- **Rationale**: não precisa de patch Harmony, e o código fica isolado num arquivo.
- **Alternatives considered**: Postfix em `MoonWalker.Update`, descartado porque roda uma vez por
  personagem (local e remotos) e exigiria filtrar o local.

## R6. Conflito do Ctrl com o jogo

- **Hipótese**: nenhum `Binding` de `MoonInput` usa Ctrl. Não dá para confirmar pelo dump, porque as
  teclas são definidas em runtime.
- **Verificação**: no primeiro teste, percorrer por reflexão as propriedades estáticas de
  `MoonInput` do tipo `Binding` (são 26 propriedades separadas, não há coleção) e checar se o array
  `keys` contém `Key.LeftCtrl` ou `Key.RightCtrl`. Não usar `KeyLabel`, que pode vir traduzido. Se
  alguma usar Ctrl, revisar o edge case da spec.

## R7. Versão do mod

- Esta feature muda o comportamento local, mas não o estado compartilhado (o estado da bota já é
  sincronizado pelo jogo).
- **Decision**: cada release sobe o MINOR de `BetterMoonSweeperMain.Version` uma vez. Se 002 e 003 saírem
  juntas, a versão vai para 0.2.0 uma vez só; se saírem separadas, a primeira vai para 0.2.0 e a
  segunda para 0.3.0.
