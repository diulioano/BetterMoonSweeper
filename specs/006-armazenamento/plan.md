# Plan 006

- src/Upgrades.cs: 4 UpgradeDef novos (ReceptorGrande, SiloGrande, EstacaoGrande, EsteiraTurbo).
- src/Armazenamento.cs: lê a base uma vez e grava base × nível, try/catch por propriedade.
- src/Patches.cs: StepProductionPatch chama Armazenamento.Aplicar.
- Versão 0.5.0 (BetterMoonSweeperMain.Version, README).
