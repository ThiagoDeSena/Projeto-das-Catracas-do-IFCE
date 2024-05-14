## Projeto de Acessibilidade ao Campus do IFCE de Maracanaú com Raspberry Pi e .NET
![imagem do projeto](https://github.com/ThiagoDeSena/Programa-da-Catraca-1/assets/110785400/40cb593d-28f2-4287-9651-a0a09c53fa92)


![Badge Concluído](http://img.shields.io/static/v1?label=STATUS&message=CONCLUÍDO&color=GREEN&style=for-the-badge)
![Badge Linguagem](http://img.shields.io/static/v1?label=PLATAFORMA&message=.NET&color=purple&style=for-the-badge)
![Badge Linguagem](http://img.shields.io/static/v1?label=LINGUAGEM&message=CSharp&color=violet&style=for-the-badge)
![Badge Linguagem](http://img.shields.io/static/v1?label=API&message=Intranet&color=darkgreen&style=for-the-badge)

<p>
<img src="https://static-00.iconduck.com/assets.00/raspberry-pi-icon-2048x2048-p0y4r07x.png" alt="Raspberry" width="40" height="40"/>
<img src="https://neosmart.net/blog/wp-content/uploads/2019/06/dot-NET-Core.png" alt="dotnet" width="40" height="40"/>
<img src="https://e7.pngegg.com/pngimages/328/221/png-clipart-c-programming-language-logo-microsoft-visual-studio-net-framework-javascript-icon-purple-logo.png" alt="c#" width="40" height="40"/>
</p>

## Descrição do projeto 

<p align="justify">
Este projeto visa implementar um sistema de controle de acesso no campus do IFCE de Maracanaú utilizando Raspberry Pi e tecnologia .NET. 
O objetivo principal é promover a acessibilidade ao campus, permitindo a entrada de pessoas autorizadas de forma rápida e eficiente, além de fornecer 
informações relevantes ao guarda de segurança responsável pelo monitoramento da entrada.

</p>

## Funcionalidades

:heavy_check_mark: `Leitura de cartão de acesso via Raspberry Pi:` Utilização de Leitor de cartão RFID.

:heavy_check_mark: `Integração com a API do IF para verificar a validade do cartão`  API DE TESTE - [API para teste](https://api.intranet-h.maracanau.ifce.edu.br/swagger/index.html) | API DE PRODUÇÃO - [API de produção](https://api-v2.intranet.maracanau.ifce.edu.br/)

:heavy_check_mark: `Controle de acesso à catraca:`  Liberação da catraca para usuários autorizados e bloqueio da catraca para usuários não autorizados.

:heavy_check_mark: `Exibição de informações do usuário na tela do monitor do guarda:` O programa manda os dados cadastrados do usuário(nome, foto, matricula e curso) que passou pela catraca para o monitor do guarda da recepção.

## Hardware

- Raspberry Pi 
- Leitor de cartão RFID
- Catraca
- Monitor

## Software

- Sistema operacional Raspbian
- Linguagem de programação C#
- Framework .NET
- API do IFCE

## Funcionamento

- O usuário aproxima o cartão de acesso do leitor RFID conectado ao Raspberry Pi.
- O Raspberry Pi lê o código do cartão e o envia para a aplicação .NET.
- A aplicação .NET consulta a API do IF para verificar se o cartão é válido e se o usuário está autorizado a entrar no campus.
- Se o cartão for válido, a aplicação .NET envia um sinal para o Raspberry Pi liberar a catraca.
- A catraca é liberada e o usuário pode entrar no campus.
- A aplicação .NET também exibe as informações do usuário na tela do monitor do guarda, como nome, foto e curso.
- Se o cartão não for válido, a catraca permanece bloqueada e o usuário não pode entrar no campus.

## Explicação do Projeto

[Assista o vídeo aqui para entender o funcionamento do projeto](https://www.linkedin.com/posts/thiago-de-sena-ab5b09179_raspberry-dotnet-net-activity-7188267112476397568-UBgK?utm_source=share&utm_medium=member_desktop)


## Benefícios


:heavy_check_mark: `Acessibilidade:`Permite a entrada rápida e eficiente de pessoas autorizadas no campus.

:heavy_check_mark: `Segurança:`Controla o acesso ao campus e impede a entrada de pessoas não autorizadas.

:heavy_check_mark: `Transparência:` Fornece informações relevantes ao guarda de segurança sobre o usuário que está entrando no campus.

:heavy_check_mark: `Modernização:` Implementa uma solução tecnológica moderna e eficiente para o controle de acesso.



## Público-alvo

- Comunidade interna e externa que frequenta o campus do IFCE de Maracanaú.

## Desenvolvedores

[<img src="https://avatars.githubusercontent.com/u/110785400?v=4" width=115><br><sub>Thiago De Sena</sub>](https://www.linkedin.com/in/thiago-de-sena-ab5b09179/)
