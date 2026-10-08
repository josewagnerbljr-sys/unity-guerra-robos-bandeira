# 📘 Manual do Jogo — Guerra dos Robôs Nordestinos

**Captura de Bandeira 3D (Unity)** · Versão 1.0 · Outubro de 2026

Manual de instruções completo: como baixar, instalar, jogar, usar os controles e resolver problemas comuns.

---

## 1. Visão geral

Dois times de robôs, **VERMELHO** (você) e **VERDE** (controlado pela IA), disputam uma partida de **5 minutos** em uma arena no sertão nordestino.
Cada time tem **4 robôs**, um de cada tipo. Você controla um deles; os outros 7 são controlados pela IA.

**Objetivo:** pegar a bandeira do time inimigo, levá-la até a sua base e, ao mesmo tempo, **eliminar o maior número possível de robôs inimigos**.

---

## 2. Requisitos

| Item | Requisito |
|---|---|
| Unity Hub | Versão atual |
| Unity Editor | **6000.6.4f1** (versão em que o jogo foi testado) |
| Template | Universal 3D (URP) |
| Sistema | Windows 64 bits |
| Git (opcional) | Para clonar o repositório |

---

## 3. Como baixar

**Opção A – Git (recomendada)**

No Git Bash ou terminal:

`git clone https://github.com/josewagnerbljr-sys/unity-guerra-robos-bandeira.git`

**Opção B – ZIP**

1. Abra https://github.com/josewagnerbljr-sys/unity-guerra-robos-bandeira
2. Clique no botão verde **Code** e depois em **Download ZIP**.
3. Extraia o ZIP em uma pasta do seu computador, de preferência com caminho curto e sem acentos.

---

## 4. Como instalar e abrir

1. Abra o **Unity Hub**.
2. Instale o Editor **6000.6.4f1** em *Instalações*, se ainda não tiver (marque *Android Build Support* se quiser gerar APK).
3. Em *Projetos*, clique na seta ao lado de **Adicionar** e escolha **Adicionar projeto do disco**.
4. Selecione a pasta do jogo (a que contém `Assets`, `Packages` e `ProjectSettings`).
5. Clique no projeto para abrir e espere o Unity importar os arquivos (a primeira vez pode levar alguns minutos).

### Configuração inicial (só na primeira vez)

1. Menu **Edit → Project Settings → Player**. Em *Other Settings*, ache **Active Input Handling** e escolha **Both**. Aceite reiniciar o Unity.
2. Menu **Robo Flag Wars → 1 - Criar cena do jogo**. Cria a cena `Assets/Scenes/RoboFlagWars.unity`.
3. Menu **Robo Flag Wars → 2 - Configurar projeto para celular**. Define orientação paisagem e espaço de cor Linear.

---

## 5. Como jogar (passo a passo)

1. Clique na aba **Game** e aperte o botão **▶ Play** no topo do Unity.
2. Na tela de escolha, clique no robô que quer controlar (veja a seção 7).
3. A partida começa na hora. Você está na **base vermelha** (lado esquerdo da arena), olhando para o lado inimigo.
4. Mova-se, mire e atire nos robôs verdes.
5. Corra até a **bandeira verde** (na base do outro lado), encoste nela para pegar e leve até a **sua base** para resgatá-la.
6. Quando o cronômetro chegar a **00:00**, o jogo mostra o resultado. Clique em **JOGAR DE NOVO** para recomeçar.

---

## 6. Controles

| Ação | PC (teste no Editor) | Celular (na tela) |
|---|---|---|
| Mover | **W A S D** ou setas | Joystick à esquerda |
| Mirar / olhar | Segurar o **botão direito do mouse** e mover | Arrastar o dedo na metade direita da tela |
| Laser | **Espaço** (segurar) | Botão vermelho **LASER** (segurar) |
| Habilidade especial | **E** | Botão azul à esquerda do LASER |

> No PC, clique uma vez dentro da aba **Game** para ela receber o teclado.
> Os controles de toque estão implementados, mas ainda não foram testados em um aparelho Android.

### Mira

A **mira (+) fica no centro da tela** e ela **fica vermelha** quando está sobre um inimigo. Os **lasers saem dos dois olhos do robô** e convergem no ponto onde a mira aponta. Basta apontar a mira e segurar o laser.

---

## 7. Os 4 robôs

| Robô | Tipo | Vida | Velocidade | Especial | Recarga |
|---|---|---|---|---|---|
| **Zé da Peixeira** | Bípede humanoide | 120 | Média (6) | **Peixeirada** | 6 s |
| **Caramelo Arretado** | Quadrúpede canino | 85 | Alta (8,5) | **Mijada no Poste** | 9 s |
| **Lula Lampião** | Cefalópode | 100 | Baixa (5,5) | **Jato de Tinta** | 10 s |
| **Camaleão Mandacaru** | Réptil camaleão | 90 | Boa (6,8) | **Camuflagem** | 12 s |

### Habilidades especiais

- **Peixeirada (Zé da Peixeira):** golpe de faca na frente do robô, até cerca de 2,7 m e num arco de 75° para cada lado. Causa **50 de dano** em quem for atingido. Ótima para o corpo a corpo.
- **Mijada no Poste (Caramelo):** deixa uma poça que **atrasa os inimigos (velocidade cai pela metade)** e causa **6 de dano por segundo**, durante **7 s**. Perto de um **poste de luz** (até 3,5 m), a poça fica **bem maior** (raio de 5,5 m contra 2,8 m).
- **Jato de Tinta (Lula Lampião):** lança uma nuvem de tinta 3,5 m à frente, com raio de 4 m, por **5 s**. Inimigos dentro dela ficam mais lentos, levam **4 de dano por segundo** e **perdem a visão** (a tela do jogador escurece e a IA enxerga só a curta distância).
- **Camuflagem (Camaleão Mandacaru):** o robô fica **invisível para os inimigos por 6 s** (eles só o veem se chegarem a menos de 4 m). A IA inimiga também deixa de mirar nele à distância.

---

## 8. Regras da partida

| Regra | Valor |
|---|---|
| Duração | 5:00, em contagem regressiva |
| Dano do laser | 9 por disparo (cerca de 5 disparos por segundo) |
| Renascimento | 4 s após morrer, na base do próprio time (2 s de proteção) |
| Pegar a bandeira | Encostar nela (até 2,2 m) |
| Resgatar a bandeira | Chegar até 3,5 m da própria base carregando a bandeira inimiga |
| Quem carrega a bandeira | Anda 10% mais devagar |
| Bandeira derrubada | Cai no chão quando o portador morre; volta sozinha à base após **12 s**, ou na hora se alguém do time dono encostar nela |
| Buracos | Há **8 buracos**. Cair em um faz o robô **tropeçar**: fica caído por 1,6 s e perde 5 de vida |

### Como se ganha

1. Vence o time com **mais kills** (robôs inimigos eliminados) ao final dos 5 minutos.
2. Se as kills empatarem, vence quem **resgatou a bandeira primeiro**.
3. Se nada disso decidir, a partida termina **empatada**.

> **Atenção:** quem pega a bandeira primeiro mas termina com **menos kills perde** a partida.
> Resgatar a bandeira **não encerra** a partida. A bandeira volta à base e o jogo continua.

---

## 9. Tela do jogo (HUD)

- **Topo, centro:** cronômetro (fica vermelho nos últimos 30 s).
- **Topo, esquerda:** placar do time **VERMELHO** (kills e bandeiras).
- **Topo, direita:** placar do time **VERDE** (kills e bandeiras).
- **Abaixo do placar:** mensagens das falas e eventos dos robôs.
- **Centro:** avisos grandes (bandeira pega, bandeira caiu, bandeira resgatada).
- **Centro da tela:** mira (+), vermelha sobre inimigos.
- **Baixo, esquerda:** joystick de movimento.
- **Baixo, direita:** botões **LASER** e **ESPECIAL** (o especial mostra a contagem de recarga).

Os robôs **falam em nordestinês** em balões sobre a cabeça: ao nascer, ao eliminar alguém, ao morrer, ao tropeçar, ao pegar a bandeira e ao usar a habilidade.

---

## 10. Dicas de jogo

- **Não corra de cabeça para a bandeira.** Elimine primeiro quem está perto: kills decidem a partida.
- Use os **postes de luz** com o Caramelo para a poça de mijo ficar enorme e travar a passagem inimiga.
- Com o **Camaleão**, ative a camuflagem **antes** de pegar a bandeira para voltar sem ser atacado.
- Desvie dos **buracos** durante a fuga. Tropeçar com a bandeira é derrota quase certa.
- Cuidado com a **tinta**: se a tela escurecer, saia da nuvem antes de atirar.
- Use as **coberturas** (blocos de adobe) para se proteger dos lasers.
- O especial recarrega sozinho: use sempre que ficar disponível.

---

## 11. Jogar no celular (Android)

1. No Unity Hub, instale o módulo **Android Build Support** para o Editor 6000.6.4f1.
2. No Unity: **File → Build Settings**, escolha **Android** e clique em **Switch Platform**.
3. Em *Project Settings → Graphics → Always Included Shaders*, adicione: `Universal Render Pipeline/Lit`, `Skybox/Procedural` e `Sprites/Default`.
4. No celular, ative **Opções do desenvolvedor** e **Depuração USB** e conecte o aparelho ao PC.
5. Clique em **Build And Run**.

---

## 12. Problemas comuns

| Problema | Solução |
|---|---|
| O Hub diz "adicione um projeto válido" | Selecione a pasta que contém `Assets`, `Packages` e `ProjectSettings`. Se faltar `ProjectSettings/ProjectVersion.txt`, abra o projeto direto pelo `Unity.exe` do Editor com o parâmetro `-projectPath` |
| Unity abre em **Safe Mode** | Há erro de script. Abra o **Console**, corrija o erro vermelho e clique em *Exit Safe Mode* |
| Menu **Robo Flag Wars** não aparece | Os scripts não compilaram. Veja o Console |
| Teclado não responde no Play | Defina **Active Input Handling = Both** e clique dentro da aba **Game** |
| Tudo aparece rosa em build | Adicione os shaders em *Always Included Shaders* (seção 11) |
| Avisos de token / Unity ID no Console | São da conta Unity, não afetam o jogo |
| Aviso sobre Shader Graph ou HDRP | Vêm do template, podem ser ignorados |

---

## 13. Estrutura do projeto

- `Assets/RoboFlagWars/Scripts` — todo o código do jogo (partida, arena, robôs, IA, câmera, interface).
- `Assets/RoboFlagWars/Editor` — menu **Robo Flag Wars** do Editor.
- `Assets/Scenes/RoboFlagWars.unity` — cena criada pelo menu.
- `README.md` — apresentação rápida do projeto.

---

## 14. Assinatura

Documento elaborado por

**Consultoria & Mentoria Blanco**

Manual de instruções do jogo *Guerra dos Robôs Nordestinos — Captura de Bandeira 3D*
Projeto de aprendizado – Desafio DIO, Trilha Unity.
