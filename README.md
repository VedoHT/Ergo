# C# + Windows Form + Firebase + Extensão de navegador (html/css/javascript)
# Trabalho de TCC (Final Paper)
  Este projeto contém um conjunto de códigos relacionados ao meu trabalho de conclusão de curso (TCC).

## Tecnologias utilizadas 
  - C#;
  - Firebase;
  - Windows Form;
  - LucidChart;
  - CSS;
  - HTML;
  - Javascript;

**OBSERVAÇÃO**
  Neste README não serão detalhados os módulos e códigos em questão. Somente será explicado e demonstrado
  o software em si e suas funcionalidades.

## Diagramas de casos de uso
  O sistema possui somente um caso de uso: usuário utilizador, onde o mesmo
  fará uso do sistema em todas as etapas. A ferramenta utilizada para a criação deste
  modelo foi o LucidChart.

  **Usuário Utilizador**
  
  ![](https://i.imgur.com/PLkH4Bf.png)

## Diagramas de Entidade-Relacionamento (ER)
  A modelagem da figura abaixo é composta pela entidade Usuarios que contém o
  identificador único chamado Username, além de possuir as entidades
  NameComplete, Password e BornDate. A entidade UsersComplete possuímos
  Username como identificador único que relaciona a entidade Usuarios, além de
  possuir Email, NameComplete, Phone, ProfilePic e BornDate. Já a entidade
  UsersConfig possui Username como identificador único que relaciona a Usuarios,
  além de possuir as Idioma, Minimizar, PermitirNotif e TempoPe. Também se possui
  a entidade Graficos, onde possuímos Username como identificador único que se
  relaciona com a entidade Usuarios e também Count como identificador único, além
  de possuir NomeGrafico, TempoPe, TempoSentado, Pizza.
  Por fim se possui a entidade Pdfs onde se possui somente a entidade PdfErgo.
  
  ![](https://i.imgur.com/qAtr56v.png)

## Firebase
  O Firebase Realtime Database é um banco de dados NoSQL hospedado na
  nuvem que oferece sincronização de dados em tempo real entre clientes (aplicativos
  da Web e móveis) e o banco de dados. Como o nome sugere, o banco de dados
  armazena e recupera dados em tempo real, o que significa que quaisquer alterações
  nos dados são refletidas instantaneamente em todos os dispositivos conectados.
  Sendo assim, este formato de banco devido a sua facilidade se aplicava corretamente 
  ao projeto em questão.

## Telas do Sistema
  Nesta seção será apresentado as telas do sistema desenvolvido,
  demonstrando seu fluxo normal de operação. Sendo assim, será descrito os passos
  do usuário ao realizar o fluxo de utilização do sistema.

  **Login**
  Nesta tela é demonstrado o login do sistema, que é necessário para o acesso ao mesmo.
  Ao incluir usuário e senha corretos, o sistema permite a entrada do usuário com o botão
  "Entrar".
  
  ![](https://i.imgur.com/gJE61xW.png)

  Porém, se entrar com os dados inválidos o sistema demonstra uma pequena
  janela informativa, representada na imagem abaixo.
  
  ![](https://i.imgur.com/2Mt8TIQ.png)

  Caso não possua um cadastro, existe o botão de “Registrar-se”, que leva o
  usuário para a figura da seção abaixo: "Registrar-se".


  **Registrar-se**
  Nesta tela é demonstrado o registro do sistema, onde o usuário
  colocará os respectivos dados solicitados para criar um cadastro.
  
  ![](https://i.imgur.com/Bg5iPT2.png)

  Ao efetuar seu cadastro, será apresentado um pequeno popup sinalizando a eficácia
  do registro, redirecionando o usuário para a tela principal do sistema.

  ![](https://i.imgur.com/ngYqP2U.png)


  **Tela Principal**
  Após efetuar o registro ou realizar a autenticação da seção, o usuário será
  encaminhado para a tela principal do sistema, onde fica a posição dos botões que
  encaminham para seus respectivos menus.

  ![](https://i.imgur.com/au3OjUg.png)


  **Configurações**
  Ao clicar na tela de configuração, o usuário com seção autenticada, será
  encaminhado para a aba de “Configurações”. Ao carregar a tela, o usuário terá 
  configurações que podem ser personalizadas, como o idioma do programa 
  (sendo disponível Português e inglês), se permite notificações no
  Windows, se o programa deve ser minimizado automaticamente caso o mesmo 17
  estiver em segundo-plano, além da opção principal que se refere à quando a
  notificação de lembrete será apresentada no Windows, devido ao propósito do
  programa em indicar o momento que se deve dar uma pausa no uso do computador.

  ![](https://i.imgur.com/BwG5IPP.png)


  **Perfil**
  Clicando na aba de “Perfil” o usuário com seção autenticada será
  encaminhado para a respectiva tela. Nesta tela o usuário possui alguns de 
  seus dados já preenchidos (Usuário, Nome Completo e Data de nascimento) 
  que foram inseridos através de seu registro.
  Também se possui novos campos para realizar o preenchimento (não obrigatório),
  que são o E-mail e Telefone, além da foto de perfil do mesmo.
  O botão de “Selecionar foto” representa a funcionalidade de escolha de fotos
  na máquina pessoal, a mesma é redimensionada com os limites do quadrado a
  esquerda do botão.

  ![](https://i.imgur.com/mlOlHT1.png)


  **Cronômetro**
  Ao clicar nesta aba, o usuário com seção autenticada será encaminhado para
  a aba de “Cronometragem”. Nesta aba o usuário
  possui a visualização dos cronômetros que são acumulados os tempos que o
  mesmo se encontra em pé ou sentado. As configurações no painel de
  “Configurações” são relacionadas ao botão de “Gerar Gráficos”, assim como o tempo
  dos cronômetros.

  ![](https://i.imgur.com/w3BeMGR.png)

  Ao possuir alguma atividade no “Tempo sentado” e em “Tempo não sentado”,
  é possível gerar um gráfico no botão “Gerar gráfico”, com isso será apresentado uma
  pequena janela indicando que o mesmo foi gerado.

  ![](https://i.imgur.com/GITB83L.png)


  **Gráficos**
  Ao clicar na aba de “Gráficos”, o usuário com seção autenticada será
  encaminhado para a aba de “Gráficos”.

  ![](https://i.imgur.com/kEY6OOv.png)

  Quando carregada, a aba não possuirá nenhuma informação cadastrada caso
  o usuário não tenha realizado a geração de um gráfico.
  Porém será apresentado no painel “Lista de Gráficos Gerados” os gráficos gerados
  na aba anterior, sendo nomeadas com o indicativo da data de geração. Dependendo da configuração 
  selecionada na aba anterior, será demonstrados gráficos diferentes, sendo as duas opções de 
  Pizza ou de Colunas, além também de possuir um pequeno parágrafo com a leitura do gráfico.

  Pizza
  ![](https://i.imgur.com/6DkYccU.png)

  Colunas
  ![](https://i.imgur.com/FVwe5bW.png)


  **Dicas E-book**
  Ao clicar na aba de “Dicas E-book”, o usuário com seção autenticada será
  encaminhado para a aba de “Dicas E-book”.

  ![](https://i.imgur.com/TOLGskc.png)


  ## Extensão de navegador
  Foi também criado de acompanhamento uma extensão de navegador que utiliza de um cronômetro 
  interno para rodar ao serviço de cronometragem do programa desktop. 
  OBS: Foi criado somente o visual e o script de cronometragem, devido ao tempo limite do BD
  não foi integrado ao mesmo, sendo assim, ele funciona a parte e sem integração ao desktop.

  ![](https://i.imgur.com/VebUGA5.png)
  
