Zdravím Romana a Štěpána a jakéhokoliv dalšího hodnotitele dané aplikace.

Adresář se simulovanými fotografiemi kontaktů je v binu, takže jej na GitHubu nenaleznete, ale kompletní VS solution je možno stáhnout zde:
https://www.uschovna.cz/zasilka/UK4XPJX8NRLUN3UN-29A/

Build dává varování kvůli konfliktu .NET 9 a .NET Framework (eWay-CRM API), což už nemám čas předělávat, ale aplikace funguje.

Ač jsem projevil zájem o spolupráci v SQL (inzerát Integrační specialista (SQL + jakýkoliv programovací jazyk)), kupodivu mi došlo testovací zadání jen na .NET, v podstatě většinou UX/UI/FE s něco málo BE, i když všude [píši](https://www.linkedin.com/in/miroslav-hustak/), že na UI/UX nemám talent a FE dělám jen z donucení, neb jsem backender. 

Tož jsem sice dané zadání zpracoval, ale pokud byste mi ještě poslali zadání pro SQL, příp. i s ADO.NET (např. jestli nepotřebujete přehodit něco z XML, JSONu či Excelu do DB, kde dá demonstrovat práce s type providers), budu rád.

Z výše uvedených důvodů je zbytečné hodnotit můj kód v XAML a grafickou úroveň aplikace, vypadá jak vypadá. Prosím zaměřte se hlavně na BE a přechod FE/BE.

Z formálního hlediska aplikace splňuje zadání (C# plus .NET technologie dle vlastní úvahy), neb C# tam je, ale jen pro okrajové interoperatibilní záležitosti, navíc většinou generovaný danou technologií (moje jsou tam snad jen 2-3 řádky) či psaný někým jiným.

Hlavní .NET technologie dle mé úvahy :-) použité v aplikaci:
* Elmish.WPF (MVU) https://github.com/elmish/Elmish.WPF
* XAML
* F# https://github.com/MiroslavHustak/FAQ
* Thoth.Json.Net https://thoth-org.github.io/Thoth.Json/

CE builders, Option/Result extentions jsou mé vlastní "knihovny", takže je vkládám do VS solutions tak, jak jsou, bez ohledu na to, kolik se toho nakonec využije.

Rutinní kód jsem do aplikace z časových důvodů nedával. To jest v aplikaci není:
* logging
* podrobné členění exceptions (svůj error handling jsem tady velmi zjednodušil)
* do/dto/transformační layer u serializace/deserializace na/z HD, v kódu už jsou dvě podobné DDD, to bych se opakoval
* connectivity listener, řešení blokace kontrolek / cancellation v případě přerušení připojení k CRM
* úprava textu v informačním textboxu ("plácnul" jsem tam celý record tak, jak je, i s SCDUs používaných pro Type DD)
* testování - tady by v úvahu připadalo snad jen PBT, i když vzhledem k tomu, že používám reflection-free Thot.Json.Net, problémy by neměly být (stress testing jsem samozřejmě provedl)
* omezení počtu položek listboxu (seznamu posledně vyhledávaných adres)

Pokud potřebujete vidět, jak jsem to kdysi řešil, kód naleznete např. tady:

https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/Connectivity/Connectivity.fs
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/XElmish/ActorModels.fs
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/tree/master/Logging
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/ExceptionHandling/ExceptionHandlers.fs

Používal jsem copilota?

Sice ano, ale kód v aplikaci je většinou můj vlastní "pre-LLM" kód recyklovaný z těchto aplikací:
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI
[https://github.com/MiroslavHustak/Unique_Identifier_And_Metadata_File_Creator](https://github.com/MiroslavHustak/Unique_Identifier_And_Metadata_File_Creator/tree/master/Unique_Identifier_And_Metadata_File_Creator.Models/XElmish)

Code review

Na to bych potřeboval několikadenní odstup, takže to do termínu odevzdání nestihnu.
Code review pomocí LLM mé kodovací standardy zapovězují :-). 

Mé kodovací standardy:

https://github.com/MiroslavHustak/FSharp-Coding-Guidelines

Přeji všem hodnotitelům krásný den s F# (syntaxe a konstrukce jazyka F# je geniálně jednoduchá, funkcionální kód pochopí i ten, kdo ještě F# nikdy neviděl, těžší věci, jako třeba free monad, point-free syntax, applicative functor či funkce z FSharpPlus, v kódu nejsou).


