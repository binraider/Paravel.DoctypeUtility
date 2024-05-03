# Paravel.DoctypeUtility
1. A utility for comparing Doctypes on Different Umbraco sites.
2. This is written for Umbraco 13. 
3. Basically you install it on sites that  you want to compare Doctypes
4. A dashboard will appear in the settings next to "Telemetry Data"
5. You must add the sites in appsettings.json in the form stated below

```
"DTUSettings": {
  "AuthToken": "6c1fddb5-003c-4059-bbe4-8be016e12e5f",
  "Sites": [
    {
      "Name": "Published_site",
      "Url": "http://umbraco13a4.local"
    },
    {
      "Name": "Self",
      "Url": "https://localhost:44317"
    },
    {
      "Name": "Some_other_site",
      "Url": "https://localhost:44324"
    }
  ]
}

```
6. The "Name" field is used in the angular
