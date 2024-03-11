# Labbinstruktioner 12/3-2024 
## PostN�rd Systems
PostN�rds system f�r pakethantering "PostN�rd Systems" har efter en st�rre refaktorering av en konsult fr�n det helt fiktiva 
konsultbolaget TiitoIvri b�rjat ha stora problem med sina tj�nster.
Buggar och of�rv�ntade beteenden som tidigare inte f�rekommit rapporteras nu in dagligen och skapar stor frustration bland kunder och medarbetare
samt inneb�r stora kostnader f�r manuell handl�ggning av paketskickande.

## Uppdragsbeskrivning
Ni har f�tt i uppdrag att verifiera att koden g�r det den ska utifr�n de krav som presenteras nedan.
Enhetstester som testar logiken och det f�rv�ntade beteendet i stort efterfr�gas.

De tv� tj�nsterna som enhetstester ska testas �r:
1. PackageCalculationService
2. PackageService

Eventuella buggar som identifieras b�r �ven �tg�rdas.
-------------------------------------------------------------------------
### PackageCalculationService
Det �r av st�rsta vikt att portober�kningen fungerar p� korrekt s�tt, d� den �r grunden f�r ett verktyget som
postkunder anv�nder f�r att ber�kna vikten p� sina paket, och som �r grunden f�r hela postsystemets inkomstssystem.

F�r att underl�tta f�r sina kunder har posten tagit beslutet att priserna f�r allt porto �r i hela kronor.

#### Startkostnad
Minsta kostnaden f�r porto �r 22 kr, vilket �r startavgiften f�r samtliga paket.

#### Viktens p�verkan p� priset
Till�gg utifr�n vikt ber�knas p� f�ljande s�tt:
Tabell f�r prisber�kning utifr�n vikt:
100g > vikt <= 500g = 33kr
500g > vikt <= 1000g = 52kr
1000g > vikt <= 2000g = 78kr
2000g > vikt <= 5000g = 155kr
5000g > vikt <= 12500g = 251kr,
12500g > vikt <= 25000g = 373kr
Paket som v�ger �ver 25kg kan inte skickas med denna tj�nst utan m�ste skickas via postens fettdrygtpakettj�nst.

#### Storlekens p�verkan p� priset
�ven storleken p� paketen kan ha en inverkan p� kostnadsber�kningen.
Om l�ngden, bredden eller h�jden faller inom intervallet >= 125cm <= 175cm tillkommer ett drygattfraktatill�gg p� 111kr.
Om paketet har exakta m�tten 80x60x20cm vilket �r PostN�rds standardm�tt p� sin popul�ra "svinbral�da"-produkt s� tas 12.5% 
av totalkostnaden bort f�r frakten p� paket vars vikt �r >=5000g.
Paket som har en l�ngd, h�jd eller bredd som �r st�rre �n 175cm kan skickas med denna tj�nst, utan m�ste hanteras via postens fettdrygtpakettj�nst.
Om l�ngd + bredd + h�jd �verstiger 300cm kan paketet inte skickas med denna tj�nst, utan m�ste hanteras via postens fettdrygtpakettj�nst.

#### Destinationens p�verkan p� priset
Ska paketet utomlands tillkommer ett utomlandstill�gg p� 31 kr, 
eller 1.5% av vikten i gram om det �verstiger 31 kr (0.015 * 2500g = 37.5 = 38kr) avrundat till n�rmaste hela krona.

#### I de fall d�r tj�nsten inte kan hantera paketen d� det st�r utanf�r kraven eller inte �r ett korrekt state
s� ska ett passande excetpion kastas. Klienten som brukar tj�nsten ska ta h�jd f�r det.