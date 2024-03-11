# Labbinstruktioner 12/3-2024 
## PostNörd Systems
PostNörds system för pakethantering "PostNörd Systems" har efter en större refaktorering av en konsult från det helt fiktiva 
konsultbolaget TiitoIvri börjat ha stora problem med sina tjänster.
Buggar och oförväntade beteenden som tidigare inte förekommit rapporteras nu in dagligen och skapar stor frustration bland kunder och medarbetare
samt innebär stora kostnader för manuell handläggning av paketskickande.

## Uppdragsbeskrivning
Ni har fått i uppdrag att verifiera att koden gör det den ska utifrån de krav som presenteras nedan.
Enhetstester som testar logiken och det förväntade beteendet i stort efterfrågas.

De två tjänsterna som enhetstester ska testas är:
1. PackageCalculationService
2. PackageService

Eventuella buggar som identifieras bör även åtgärdas.
-------------------------------------------------------------------------
### PackageCalculationService
Det är av största vikt att portoberäkningen fungerar på korrekt sätt, då den är grunden för ett verktyget som
postkunder använder för att beräkna vikten på sina paket, och som är grunden för hela postsystemets inkomstssystem.

För att underlätta för sina kunder har posten tagit beslutet att priserna för allt porto är i hela kronor.

#### Startkostnad
Minsta kostnaden för porto är 22 kr, vilket är startavgiften för samtliga paket.

#### Viktens påverkan på priset
Tillägg utifrån vikt beräknas på följande sätt:
Tabell för prisberäkning utifrån vikt:
100g > vikt <= 500g = 33kr
500g > vikt <= 1000g = 52kr
1000g > vikt <= 2000g = 78kr
2000g > vikt <= 5000g = 155kr
5000g > vikt <= 12500g = 251kr,
12500g > vikt <= 25000g = 373kr
Paket som väger över 25kg kan inte skickas med denna tjänst utan måste skickas via postens fettdrygtpakettjänst.

#### Storlekens påverkan på priset
Även storleken på paketen kan ha en inverkan på kostnadsberäkningen.
Om längden, bredden eller höjden faller inom intervallet >= 125cm <= 175cm tillkommer ett drygattfraktatillägg på 111kr.
Om paketet har exakta måtten 80x60x20cm vilket är PostNörds standardmått på sin populära "svinbralåda"-produkt så tas 12.5% 
av totalkostnaden bort för frakten på paket vars vikt är >=5000g.
Paket som har en längd, höjd eller bredd som är större än 175cm kan skickas med denna tjänst, utan måste hanteras via postens fettdrygtpakettjänst.
Om längd + bredd + höjd överstiger 300cm kan paketet inte skickas med denna tjänst, utan måste hanteras via postens fettdrygtpakettjänst.

#### Destinationens påverkan på priset
Ska paketet utomlands tillkommer ett utomlandstillägg på 31 kr, 
eller 1.5% av vikten i gram om det överstiger 31 kr (0.015 * 2500g = 37.5 = 38kr) avrundat till närmaste hela krona.

#### I de fall där tjänsten inte kan hantera paketen då det står utanför kraven eller inte är ett korrekt state
så ska ett passande excetpion kastas. Klienten som brukar tjänsten ska ta höjd för det.