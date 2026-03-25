# Deployment Guide

## Environment Variables

### Required (both Dev and Production)
| Variable | Description |
|----------|-------------|
| `Jwt__Key` | JWT signing key (min 32 chars) |
| `Encryption__MasterSecret` | Master encryption secret |

### Development Setup
launchSettings.json এ:
```json
"environmentVariables": {
    "Jwt__Key": "A1B1C1a1b1c1D1E1d1e1A1B1C1a1b1c1D1E1d1e1",
    "Encryption__MasterSecret": "eyfuo&^&^*$YJVOO^R(^%$%%&IOYPIUGHG"
}
```

### Production Setup
```bash
export Jwt__Key="A1B1C1a1b1c1D1E1d1e1A1B1C1a1b1c1D1E1d1e1"
export Encryption__MasterSecret="eyfuo&^&^*$YJVOO^R(^%$%%&IOYPIUGHG"
```

## Database Setup (One-time)
```sql
-- PostgreSQL SuperUser দিয়ে run করো
ALTER ROLE "EduManager" CREATEROLE;
```

## Migration
```bash
# নতুন Migration বানানোর পরে run করো
dotnet run --project src/EduManager/EduManager.Migrator
```

## Connection Strings (appsettings.json এ থাকবে)
- MasterDBConnection
- DefaultTenantConnection (Template DB)

## .gitignore এ থাকবে
- launchSettings.json
- appsettings.Development.json