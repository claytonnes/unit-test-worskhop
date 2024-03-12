# Labbinstruktioner 12/3-2024 
## PostN�rd Systems
PostN�rds system f�r pakethantering "PostN�rd Systems" har efter en st�rre refaktorering samt en del nyutvekcling av en konsult fr�n det helt fiktiva 
konsultbolaget Totietoteo Evoverory b�rjat ha stora problem med sina tj�nster.
Buggar och of�rv�ntade beteenden som tidigare inte f�rekommit rapporteras nu in dagligen och skapar stor frustration bland kunder och medarbetare
samt inneb�r stora kostnader f�r manuell handl�ggning av paketskickande.

## Uppdragsbeskrivning
Ni har f�tt i uppdrag att testa (med enhetstester) och komplettera buggar/avsaknat beteende.
Enhetstester som testar logiken och det f�rv�ntade beteendet i stort efterfr�gas.

De tv� tj�nsterna som enhetstester ska testas �r:
1. PackageCalculationService
2. PackageService

-------------------------------------------------------------------------
## UPPGIFT 1

### PackageCalculationService
Det �r av st�rsta vikt att portober�kningen fungerar p� korrekt s�tt, d� den �r grunden f�r ett verktyget som
postkunder anv�nder f�r att ber�kna vikten p� sina paket, och som �r grunden f�r hela postsystemets inkomstssystem.

- F�r att underl�tta f�r sina kunder har posten tagit beslutet att priserna f�r allt porto �r i hela kronor.

#### Startkostnad
- Minsta kostnaden f�r porto �r 22 kr, vilket �r startavgiften f�r samtliga paket.

#### Viktens p�verkan p� priset
Till�gg utifr�n vikt ber�knas p� f�ljande s�tt:
Tabell f�r prisber�kning utifr�n vikt:
100g >= vikt <= 500g = 33kr
500g > vikt <= 1000g = 52kr
1000g > vikt <= 2000g = 78kr
2000g > vikt <= 5000g = 155kr
5000g > vikt <= 12500g = 251kr
12500g > vikt <= 25000g = 373kr
- Paket som v�ger �ver 25kg kan inte skickas med denna tj�nst.

#### Storlekens p�verkan p� priset
�ven storleken p� paketen kan ha en inverkan p� kostnadsber�kningen.
- Om l�ngden, bredden eller h�jden faller inom intervallet >= 125cm <= 175cm tillkommer ett drygattfraktatill�gg p� 111kr.

- Om paketet har m�tten 80x60x20cm vilket �r PostN�rds standardm�tt p� sin popul�ra "svinbral�da"-produkt s� tas 12.5% 
av totalkostnaden bort f�r frakten p� paket vars vikt �r >=5000g.

- Paket som har en l�ngd, h�jd eller bredd som �r st�rre �n 175cm kan inte skickas med denna tj�nst

- Om l�ngd + bredd + h�jd �verstiger 300cm kan paketet inte skickas med denna tj�nst, utan m�ste hanteras via postens fettdrygtpakettj�nst.

#### Destinationens p�verkan p� priset
- Ska paketet utomlands tillkommer ett utomlandstill�gg p� 31 kr, eller 1.5% av vikten i gram om det �verstiger 31 kr (0.015 * 2500g = 37.5 = 38kr) avrundat till n�rmaste hela krona.

#### I de fall d�r tj�nsten inte kan hantera paketen d� det st�r utanf�r kraven eller inte �r ett korrekt state
- s� ska ett passande excetpion kastas. Klienten som brukar tj�nsten ska ta h�jd f�r det.

---------------------------------------------------------------------------------

## UPPGIFT 2

### PackageService
I PackageService sker hanteringen av inkommande paket som ska skickas ut till sina mottagare.
Dessv�rre hann tidigare n�mnd konsult inte implementera denna tj�nst mer �n att skapa metoddefinitionen f�r huvudmetoden, s� nu beh�ver ni ta vid och implementera den.
Er uppdragsgivare p� PostN�rd har som krav att ni �tminstone delvis ska anv�nda er av Testdriven utveckling, vilket i princip inneb�r att ni definierar testfall f�r det �nskade beteendet innan ni implementerar koden.

�nskat beteende:
- Om ett paket har betalat lika med eller h�gre porto (PaidPostFee-property p� Package-Model) �n vad som kr�vs, 
 s� ska det f�rs�ka skickas iv�g (IPackageSenderRepository.SendPackage), annars inte.

- Om ett pakets betalda porto (PaidPostFee-property p� Package-Model) understiger v�rdet som (IPackageCalculation.CalculatePostageRate) ber�knas,
returnera paketet till avs�ndaren (IPackageSenderRepository.ReturnPackage)

- Om ett paket inte lyckas skickas till mottagare, returnera det till sin mottagare (IPackageSenderRepository.ReturnPackage)

- Om ett paket inte kan returneras till sin mottagare, skicka det till "borttappat"-lagring (ILostMailRepository.SendToStorage)

- Om ett paket inte lyckas skickas till borttappat-lagring, kasta ett exception av passande slag

- Varje paket som hanteras ska alltid generera MINST tv� loggningar (ILoggerService), en LogStart och en LogEnd.

- PostN�rd framh�ver vikten att enbart EN startloggning sker, och vill att ni implementerar ett enhetstest som s�kerst�ller det.

- Om ett exception f�ngas ska det loggas (ILoggerService)

- En str�ng som beskriver slutresultatet av hanteringen ska returners i de fall d�r ett exception inte kastas.
