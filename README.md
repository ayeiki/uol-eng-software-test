# Cupom de Desconto no Carrinho

Visão geral
-----------
Implementar uma feature que valida e aplica um cupom de desconto ao total do carrinho. A fonte dos cupons é o arquivo `files/cupom.json` presente no repositório — não é necessário criar CRUD ou persistência adicional.

Instruções para o candidato
--------------------------
Antes de iniciar o exercício, faça um fork deste repositório para sua conta no GitHub. Trabalhe no fork da seguinte forma:
- Clone o fork para sua máquina local.
- Crie uma branch para sua implementação.
- Implemente a solução e escreva/execute os testes localmente.
- Faça commits e dê push da branch para o seu fork.
- Opcional: abra um Pull Request a partir do seu fork.

Não altere arquivos em NAO_ALTERAR e siga as instruções de testes descritas neste repositório.

Principais requisitos
---------------------
- Receber um código de cupom e aplicá‑lo sobre o total do carrinho.
- Não permitir mais de um cupom por carrinho.
- Suportar dois tipos de cupom:
  - Por porcentagem: aplica N% sobre o total.
  - Por valor fixo: subtrai um valor fixo; se o desconto exceder o total, o total deve ficar em zero.
- Validar existência, tipo, valor do cupom conforme o arquivo de referência.
- Escrever testes unitários cobrindo regras e cenários.
- Não alterar nenhum arquivo de teste em `NAO_ALTERAR`.

Regras de validação (resumido)
-------------------------------
- Cupom deve existir no arquivo de cupons.
- Tipo precisa ser suportado (percent/fixed).
- Valor deve ser positivo.
- Rejeitar aplicação se o carrinho já tiver um cupom aplicado.

Cálculo do desconto (exemplos)
------------------------------
- Carrinho total 200, cupom 10% => desconto 20, total final 180.
- Carrinho total 30, cupom fixo 50 => desconto 30, total final 0.

Erros e mensagens
-----------------
- Mensagens claras e determinísticas (ex.: "Cupom inválido", "Cupom expirado", "Já existe um cupom aplicado").
- Tratar inputs inválidos com validação antecipada.

Testes
------
- Cobrir cenários positivos e negativos:
  - Aplicação correta de cupom percentual e fixo.
  - Cupom maior que total => total 0.
  - Tentativa de aplicar segundo cupom => erro.
  - Cupons invalidos.


Boas práticas de implementação
-----------------------------
- Isolar a lógica de desconto para facilitar testes.
- Não alterar contratos públicos ou os testes em `NAO_ALTERAR`.
- Preferir abordagens não invasivas para não quebrar código legado.
- Evitar mudanças em interfaces; se necessário documentar e justificar no PR.
- Adicionar comentários nas decisões importantes e incluir resumo no PR.

Considerações sobre código legado
--------------------------------
- Implementar incrementalmente e de forma compatível com o código existente.
- Garantir que falhas na nova feature não comprometam fluxos existentes (defensivo).
- Evitar quebras de contratos (alteração de parametros, construtores, assinaturas de metodos).

Entrega e documentação
----------------------
- Incluir testes unitários que validem regras principais.
- Documentar no PR as decisões importantes.


O que será avaliado
-------------------
- Criatividade na solução.
- Compreensão e implementação correta dos requisitos da feature.
- Capacidade de escrever testes unitários eficazes que cubram cenários diversos.
- Habilidade em seguir boas práticas de implementação e documentação.
- Capacidade de trabalhar com código legado de forma defensiva e incremental.
- Clareza e organização na comunicação das decisões tomadas durante o desenvolvimento.




<sub><sup>dica secreta: a melhor solução não altera a `CartTotalCalculator`, mas você não perde pontos se alterar</sup></sub>
