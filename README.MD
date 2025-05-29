# Sistema de Monitoramento de Falhas de Energia

Aplicação em console para registro e acompanhamento de falhas sistemáticas na infraestrutura de TI.
## Grupo
- Gabriel Ciziks   | RM98215
- Lucca Tambellini | RM98169
- Cassio Valezzi   | RM551059
## Este sistema permite às equipes de TI:

- Registrar eventos de falha de sistemas com classificação de impacto
- Acompanhar incidentes não resolvidos
- Gerar relatórios periódicos de impacto
- Manter logs de auditoria de todas as ações

## Instruções de Execução
### Pré-requisitos

- .NET Core 3.1 ou superior

### Como Executar

- Clone o repositório
- Navegue até o diretório do projeto
- Abra com o Visual Studio
- Execute

### Dependências

- Runtime do .NET Core
- Nenhum pacote externo necessário

## Estrutura de Pastas
```
/GS_CSharp_Energia
  ├── Models/          
  │   ├── EnergiaModel.cs
  │   └── User.cs
  ├── Services/       
  │   ├── AuthService.cs
  │   └── EnergiaService.cs
  └── Program.cs
```

## Guia de Uso

- Faça login com credenciais padrão de admin (admin/admin123) ou registre novo usuário

- Opções do menu principal:

    - Registrar novas falhas
    - Visualizar falhas ativas
    - Marcar falhas como resolvidas
    - Gerar relatórios
    - Visualizar logs do sistema

- Credenciais Padrão

    - Admin: usuário admin / senha admin123
    - Livre para novos registros

## Exemplos de Uso

### Registrar nova falha:
1. Selecione "Reportar Falha"
2. Informe sistemas afetados (ex: 1,3,5)
3. Preencha data, localização e descrição
4. Selecione nível de impacto (1-4)

###  Gerar relatório:
1. Selecione "Gerar relatório"
2. Informe período (dd/MM/aaaa)
3. Defina o nome do arquivo (Colocando a extensão .txt no final) ex: relatorio_1.txt