# Integração com Microsoft Entra ID (Azure Active Directory)

## Visão Geral
Este documento descreve os passos necessários para implementar autenticação via Microsoft Entra ID (anteriormente Azure Active Directory) em nossa aplicação Financial Management.

## Pré-requisitos
- Conta no Microsoft Azure com acesso administrativo ao Entra ID
- Visual Studio ou Visual Studio Code
- Conhecimento básico de autenticação OAuth 2.0 e OpenID Connect

## Passos para Implementação

### 1. Configuração no Portal do Azure

1. Acessar o [Portal do Azure](https://portal.azure.com)
2. Navegar para "Microsoft Entra ID" > "Registros de aplicativos"
3. Clicar em "Novo registro"
4. Preencher as informações:
   - Nome: Financial Management
   - Tipos de conta suportados: Selecionar opção adequada (apenas contas da organização ou incluir contas pessoais)
   - URI de redirecionamento: Adicionar URI do frontend e backend
5. Clicar em "Registrar"
6. Anotar o ID do aplicativo (client ID) e o ID do diretório (tenant ID)

### 2. Configuração do Backend (ASP.NET Core)

1. Instalar pacotes NuGet necessários:
   ```
   Microsoft.Identity.Web
   Microsoft.Identity.Web.UI
   ```

2. Configurar o `appsettings.json`:
   ```json
   "AzureAd": {
     "Instance": "https://login.microsoftonline.com/",
     "Domain": "seudominio.com",
     "TenantId": "seu-tenant-id",
     "ClientId": "seu-client-id",
     "CallbackPath": "/signin-oidc",
     "SignedOutCallbackPath": "/signout-callback-oidc"
   }
   ```

3. Configurar serviços na `Startup.cs` ou `Program.cs`:
   ```csharp
   builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
   ```

4. Adicionar autorização aos controladores:
   ```csharp
   [Authorize]
   [ApiController]
   [Route("api/[controller]")]
   public class FinancialManagementController : ControllerBase
   ```

### 3. Configuração do Frontend (React)

1. Instalar a biblioteca MSAL:
   ```bash
   npm install @azure/msal-browser @azure/msal-react
   ```

2. Criar arquivo de configuração para autenticação:
   ```typescript
   // src/auth/authConfig.ts
   export const msalConfig = {
     auth: {
       clientId: "seu-client-id",
       authority: "https://login.microsoftonline.com/seu-tenant-id",
       redirectUri: "http://localhost:3000",
     },
     cache: {
       cacheLocation: "sessionStorage",
       storeAuthStateInCookie: false,
     },
   };

   export const loginRequest = {
     scopes: ["User.Read", "api://seu-client-id/access_as_user"]
   };
   ```

3. Configurar o provedor MSAL em `index.tsx` ou `App.tsx`:
   ```tsx
   import { MsalProvider } from "@azure/msal-react";
   import { PublicClientApplication } from "@azure/msal-browser";
   import { msalConfig } from "./auth/authConfig";

   const msalInstance = new PublicClientApplication(msalConfig);

   ReactDOM.render(
     <React.StrictMode>
       <MsalProvider instance={msalInstance}>
         <App />
       </MsalProvider>
     </React.StrictMode>,
     document.getElementById("root")
   );
   ```

4. Implementar componentes de autenticação e proteção de rotas.

## Testes e Validação

1. Verificar o fluxo de login completo
2. Testar acesso a APIs protegidas
3. Verificar o token JWT e suas claims
4. Testar o comportamento de expiração do token

## Próximos Passos

- Implementar autorização baseada em roles/claims
- Configurar políticas de acesso condicional no Azure
- Implementar renovação de token silenciosa

## Referências

- [Documentação oficial do Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)
- [Tutorial ASP.NET Core com Microsoft Identity](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-aspnet-core-webapp)
- [Biblioteca MSAL.js para React](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-react)
