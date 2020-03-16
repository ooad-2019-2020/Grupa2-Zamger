# Grupa2-Zamger
### Tema: Aplikacija za studente

![](https://i.imgur.com/9DK3QAZ.png)

##### Članovi tima:
- [Huso Hamzić](https://github.com/hhamzic1)
- [Paša Džumhur](https://github.com/PasaDzumhur)
- [Rijad Handžić](https://github.com/rhandzic1)

### Opis projekta:
Visokofunkcionalan informacioni sistem za visokoškolske ustanove koji olakšava dnevni workflow kako studenata tako i profesora. Aplikacija daje uvid u ostvarene rezultate studenta kao i mogućnosti da student prijavi ispite i odazove se na sve fakultetske obaveze i zadaće. Omogućava nastavnom osoblju da ima uvid i kontrolu nad predmetima na kojim su zaduženi kao i kreiranje ispitnih termina, zadaća za iste. Također studentskoj službi olakšava svakodnevne obaveze oko potvrda i papirologije.

### Procesi:
##### Student:
+ Student se prijavljuje na sistem sa svojim studentskim pristupnim podacima.
+ Dočekuje ga dashboard(kontrolna ploča) na kojoj ima uvid u novosti vezane za studij(termini za zadaće itd...).
+ Student ima uvid u svoj ostvareni uspjeh prilikom studiranja(po predmetu), ocjenama, broju bodova na ispitima, broju bodova na zadaćama itd...
+ Student se može prijaviti na ispit iz predmeta kojeg pohađa ako postoji otvoren termin za prijavu.
+ Student može zatražiti izdavanje potvrde od studentske službe(za regulisanje stipendije, gradskog prevoza itd...).
+ Student, ako postoji, može ispuniti anketu za određeni predmet(mišljenje o predmetu itd...).

##### Nastavno osoblje:
+ Nastavno osoblje se prijavljuje na sistem sa pristupnim podacima za nastavno osoblje(odovojeni od studentskih pristupnih podataka).
+ Nakon prijave dočekuje ih dashboard(kontrolna ploča) na kojoj mogu birati neke od mnogih funkcionalnosti i opcija.
+ Nastavno osoblje ima uvid u čitav predmet na kojem su zaduženi(uvid u rezultate svih studenata kao i sve detalje o svakom studentu pojedinačno vezane za taj konkretan predmet).
+ Nastavno osoblje može mijenjati podatke o uspjehu studenta na predmetu koji taj student pohađa, naravno ta osoba mora biti zadužena na tom predmetu(ocjenjivati studenta, mijenjati bodove na zadaćama itd...).
+ Nastavno osoblje može kreirati termine za razne fakultetske obaveze(termini za ispite, termini za zadaće, ankete itd...).
+ Nastavno osoblje ima uvid u studentske ankete vezane za predmet na kojem je ta osoba zadužena(sa anonimnim podacima studenta).

##### Studentska služba:
+ Prijavljuje se na informacioni sistem sa specijalnim pristupnim podacima.
+ Ima pristup svim podacima na sistemu(kako studentskim tako i podacima nastavnog osoblja).
+ Upravlja izdavanjem potvrda i ostalih dokumenata vezanih za fakultet.
+ Ima uvid u studentske ankete za sve predmete(može vidjeti prosječnu ocjenu svakog profesora zasebno i dobiti dojam o radu svakog od njih).
+ Odobrava studentu mijenjanje njegovih ličnih podataka itd...

### Funkcionalnosti:
+ Uvid u ostvareni uspjeh studenata.
+ Upravljanje svim fakultetskim procesima.
+ Monitoring rada nastavnog osoblja na fakultetu.
+ Olakšane fakultetske procedure koje bi inače stvarale nepotrebne gužve i uska grla na fakultetu.

### Akteri:
+ **Student** (korisnik usluga) - Ima uvid u svoje rezultate te se može odazvati na fakultetske obaveze
+ **Nastavno osoblje** - Osobe koje imaju uvid u rezultate studenata na predmetu i kreiraju fakultetske obaveze.
  + **Profesor**
  + **Asistent** - Ima manja prava od profesora(ne može ocijenjivati studente, ali im može bodovati/ocijeniti zadaću i prisustvo)
+ **Studentska služba** (administrator sistema) - Ima uvid i kontrolu nad svim fakultetskim procesima.

