# Paravel.DoctypeUtility
1. A utility for comparing Doctypes on Different Umbraco sites.
2. This is written for Umbraco 13. 
3. Basically you install it (via Nuget) on sites when you want to compare Doctypes to see if they are out of sync.
4. I am going to use it to see what doctypes are on the development environment that need to be added or amended on staging or production. 
5. It will only show doctypes that are different, or missing from local or remote.
6. Not every aspect of the doctype properties are compared, only the basic ones. This tool is a handy tool but it is not exhaustive, and does not claim to be. 
7. The local and remote doctypes are matched by their alias.
8. A dashboard will appear in the settings next to "Telemetry Data" or something like that.
9. You must add the sites you want to compare to in appsettings.json in the form stated below:

```
"DTUSettings": {
  "AuthToken": "6c1fddb5-003c-4059-bbe4-8be016e12e5f",
  "Sites": [
    {
      "Name": "Umbraco 13 A4",
      "Url": "http://umbraco13a4.local",
      "Key": "staging"
    },
    {
      "Name": "Umbraco 13 A5",
      "Url": "https://localhost:44317",
      "Key": "development"
    },
    {
      "Name": "Umbraco 13 A4 Debug",
      "Url": "https://localhost:44324",
      "Key": "local"
    }
  ]
}

```
10. The "Key" field is used in the angular to differenciate between the entries, so it has to be unique. It has no relevence to the other sites appsettings, so on another site they could be "meenie", "miny" "moe".
11. The "AuthToken" field must be shared between the sites you want to compare. Please provide your own value for this.  

---

### This is the basic view after clicking "View Diffs"
- Diff Id indicates that the ids are different
- Diff Key indicates that the Guids are different
- Diff Items indicates that there are some differences between the properties of the doctype
- Diff Vary indicates that the item is "Vary By Culture"



![The basic view](img/grab-1.png)

### This view is when you have clicked "View". 
The properties of the relevent doctype are listed below. 
You can see that although the remote and local block selected have similar properties, 
for the remote doctype the Doctype is "VaryByCulture" and two of the properties are also "VaryByCulture".

![Viewing the properties of a doctype](img/grab-2.png)

### This view is when you cannot connect:

![When you cant connect](img/grab-3.png)

