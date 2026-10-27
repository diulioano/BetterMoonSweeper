# Contrato: atalho de teclado da bota de gravidade

Interface exposta ao jogador.

| Item | Valor |
|---|---|
| Teclas | Ctrl esquerdo, Ctrl direito |
| Gatilho | Aperto (borda de descida); segurar não repete |
| Efeito | Alterna `GravityBootsOn` do jogador local pelo método nativo do jogo |
| Pré-condições | Em partida; jogador local tem a bota; não digitando; sem menu; fora de veículo ou robô; fora do modo de construção; não caído nem em outro estado ocupado (escada, descanso, viagem, carregado, voo de dev) |
| Sem pré-condição | Nada acontece, sem som e sem mensagem |
| Gamepad | Sem mudança; continua com o botão nativo |
| Tecla nativa da bota | Continua funcionando |
| Log | Uma linha `Bota de gravidade: ligada/desligada (Ctrl)` no log do MelonLoader a cada troca; `ToggleGravityBoots retornou false` se o jogo recusar a troca |
