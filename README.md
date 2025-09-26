# Financial Management Application

## Visão Geral
Uma aplicação de gerenciamento financeiro pessoal para controle de contas, cartões de crédito e faturas.

## Tecnologias Utilizadas
- **Frontend**: React, TypeScript, Material UI
- **Backend**: ASP.NET Core, C#

## Configuração do Projeto

### Pré-requisitos
- Node.js e npm
- .NET SDK 8.0 ou superior

### Instruções de Instalação
1. Clone o repositório
2. Para o frontend:
   ```bash
   cd Frontend
   npm install
   npm run dev
   ```
3. Para o backend:
   ```bash
   cd FinancialManagement
   dotnet restore
   dotnet run
   ```

## Tarefas Pendentes

### Frontend
- [ ] Adicionar funcionalidade de edição na página MonthlyBills
- [ ] Decisão: Criar ou remover a página Purchases (definir escopo)
- [ ] Remover colunas "limite" e "disponível" da página CreditCards
- [ ] Implementar autenticação via Entra ID (Azure AD)

### Backend
- [ ] Implementar autenticação via Entra ID (Azure AD)
- [ ] Desenvolver testes unitários para os serviços e controladores

### DevOps
- [ ] Preparar projeto frontend para pipeline CI/CD no Azure DevOps
  - Configurar build steps
  - Adicionar linting e testes automatizados
  - Configurar deployment para ambiente de desenvolvimento/teste/produção
- [ ] Preparar projeto backend para pipeline CI/CD no Azure DevOps
  - Configurar build e test steps
  - Configurar deployment para Azure App Service ou outro serviço adequado
  - Implementar migração automática de banco de dados

### Qualidade de Código
- [ ] Implementar testes unitários para componentes React
- [ ] Implementar testes unitários para serviços do backend
- [ ] Configurar análise estática de código

## Arquitetura

A aplicação segue uma arquitetura em camadas:

- **Frontend**: React com TypeScript, utilizando Material UI para componentes de interface
- **Backend**: ASP.NET Core API com arquitetura em camadas (Apresentação, Aplicação, Domínio, Infraestrutura)

## Contribuição
Para contribuir com o projeto, siga estas etapas:

1. Crie um branch para sua feature (`git checkout -b feature/nome-da-feature`)
2. Faça commit das suas alterações (`git commit -m 'Adiciona nova feature'`)
3. Faça push para o branch (`git push origin feature/nome-da-feature`)
4. Abra um Pull Request
