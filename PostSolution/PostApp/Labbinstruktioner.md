# Labbinstruktioner 12/3-2024 
## PostNörd Systems
PostNörds system för pakethantering "PostNörd Systems" har efter en större refaktorering samt en del nyutvekcling av en konsult från det helt fiktiva 
konsultbolaget Totietoteo Evoverory börjat ha stora problem med sina tjänster.
Buggar och oförväntade beteenden som tidigare inte förekommit rapporteras nu in dagligen och skapar stor frustration bland kunder och medarbetare
samt innebär stora kostnader för manuell handläggning av paketskickande.

## Uppdragsbeskrivning
Ni har fått i uppdrag att testa (med enhetstester) och komplettera buggar/avsaknat beteende.
Enhetstester som testar logiken och det förväntade beteendet i stort efterfrågas.

De två tjänsterna som enhetstester ska testas är:
1. PackageCalculationService
2. PackageService

-------------------------------------------------------------------------
## UPPGIFT 1

### PackageCalculationService
Det är av största vikt att portoberäkningen fungerar på korrekt sätt, då den är grunden för ett verktyget som
postkunder använder för att beräkna vikten på sina paket, och som är grunden för hela postsystemets inkomstssystem.

- För att underlätta för sina kunder har posten tagit beslutet att priserna för allt porto är i hela kronor.

#### Startkostnad
- Minsta kostnaden för porto är 22 kr, vilket är startavgiften för samtliga paket.

#### Viktens påverkan på priset
Tillägg utifrån vikt beräknas på följande sätt:
Tabell för prisberäkning utifrån vikt:
100g >= vikt <= 500g = 33kr
500g > vikt <= 1000g = 52kr
1000g > vikt <= 2000g = 78kr
2000g > vikt <= 5000g = 155kr
5000g > vikt <= 12500g = 251kr
12500g > vikt <= 25000g = 373kr
- Paket som väger över 25kg kan inte skickas med denna tjänst.

#### Storlekens påverkan på priset
Även storleken på paketen kan ha en inverkan på kostnadsberäkningen.
- Om längden, bredden eller höjden faller inom intervallet >= 125cm <= 175cm tillkommer ett drygattfraktatillägg på 111kr.

- Om paketet har måtten 80x60x20cm vilket är PostNörds standardmått på sin populära "svinbralåda"-produkt så tas 12.5% 
av totalkostnaden bort för frakten på paket vars vikt är >=5000g.

- Paket som har en längd, höjd eller bredd som är större än 175cm kan inte skickas med denna tjänst

- Om längd + bredd + höjd överstiger 300cm kan paketet inte skickas med denna tjänst, utan måste hanteras via postens fettdrygtpakettjänst.

#### Destinationens påverkan på priset
- Ska paketet utomlands tillkommer ett utomlandstillägg på 31 kr, eller 1.5% av vikten i gram om det överstiger 31 kr (0.015 * 2500g = 37.5 = 38kr) avrundat till närmaste hela krona.

#### I de fall där tjänsten inte kan hantera paketen då det står utanför kraven eller inte är ett korrekt state
- så ska ett passande excetpion kastas. Klienten som brukar tjänsten ska ta höjd för det.

---------------------------------------------------------------------------------

## UPPGIFT 2

### PackageService
I PackageService sker hanteringen av inkommande paket som ska skickas ut till sina mottagare.
Dessvärre hann tidigare nämnd konsult inte implementera denna tjänst mer än att skapa metoddefinitionen för huvudmetoden, så nu behöver ni ta vid och implementera den.
Er uppdragsgivare på PostNörd har som krav att ni åtminstone delvis ska använda er av Testdriven utveckling, vilket i princip innebär att ni definierar testfall för det önskade beteendet innan ni implementerar koden.

Önskat beteende:
- Om ett paket har betalat lika med eller högre porto (PaidPostFee-property på Package-Model) än vad som krävs, 
 så ska det försöka skickas iväg (IPackageSenderRepository.SendPackage), annars inte.

- Om ett pakets betalda porto (PaidPostFee-property på Package-Model) understiger värdet som (IPackageCalculation.CalculatePostageRate) beräknas,
returnera paketet till avsändaren (IPackageSenderRepository.ReturnPackage)

- Om ett paket inte lyckas skickas till mottagare, returnera det till sin mottagare (IPackageSenderRepository.ReturnPackage)

- Om ett paket inte kan returneras till sin mottagare, skicka det till "borttappat"-lagring (ILostMailRepository.SendToStorage)

- Om ett paket inte lyckas skickas till borttappat-lagring, kasta ett exception av passande slag

- Varje paket som hanteras ska alltid generera MINST två loggningar (ILoggerService), en LogStart och en LogEnd.

- PostNörd framhäver vikten att enbart EN startloggning sker, och vill att ni implementerar ett enhetstest som säkerställer det.

- Om ett exception fångas ska det loggas (ILoggerService)

- En sträng som beskriver slutresultatet av hanteringen ska returners i de fall där ett exception inte kastas.
