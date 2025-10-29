Implementar Cupom de Desconto no Carrinho

Os cupons válidos para aplicação estão definidos no arquivo `files/cupom.json`.
Descrição:
Implemente um endpoint na API para aplicar um cupom de desconto ao carrinho. O cupom deve ser validado e, se válido, o desconto deve ser aplicado ao total do carrinho.

Requisitos:

Criar um endpoint POST /cart/coupon que recebe um código de cupom.
Validar o cupom (exemplo: "DESC10" dá 10% de desconto, "FRETE" dá R$20 de desconto se o total for maior que R$100).
O desconto deve ser refletido no endpoint de total do carrinho.
Não permitir aplicar mais de um cupom por carrinho.
Escrever testes unitários para a lógica de cupom e para o endpoint.
Documentar a feature (README ou comentários).
Critérios de avaliação:

Clareza e organização do código.
Boas práticas de API e SOLID.
Cobertura de testes.
Validação e tratamento de erros.
Documentação e explicação das decisões.