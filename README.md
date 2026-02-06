Zdravím Romana a Štěpána a jakéhokoliv dalšího hodnotitele dané aplikace.

Pokud by byl problém s GitHubem, kompletní VS solution je možno stáhnout zde:
ttps://

Ač jsem projevil zájem o spolupráci v SQL (inzerát Integrační specialista (SQL + jakýkoliv programovací jazyk)), kupodivu mi došlo testovací zadání na C#, v podstatě většinou UX/UI/FE s něco málo BE, i když všude píši, že na UI/UX nemám talent a FE dělám jen z donucení, neb jsem backender. 

Tož jsem sice dané zadání zpracoval, ale pokud byste mi ještě poslali testovací zadání pro SQL, příp. i s ADO NET (např. jestli nepotřebujete přehodit něco z XML, JSONu či Excelu do DB), budu rád.

Z výše uvedených důvodů je zbytečné hodnotit můj kód v XAML a grafickou úroveň aplikace, vypadá jak vypadá. Prosím zaměřte se hlavně na BE a přechod FE/BE.

Z formálního hlediska aplikace splňuje zadání (C# plus .NET technologie dle vlastní úvahy), ale C# je tam jen pro okrajové interoperatibilní záležitosti, navíc většinou generovaný danou technologií (moje jsou tam snad jen 2-3 řádky) či psaný někým jiným.

Hlavní .NET technologie dle mé úvahy :-) použité v aplikaci:
* Elmish.WPF
* XAML
* F#
* Thoth.Json.Net

Rutinní kód jsem do aplikace z časových důvodů nedával, což je:
* logging
* podrobné členění exceptions (můj error handling jsem tady zjednodušil)

Rovněž tam není connectivity listener, řešení blokace kontrolek / cancellation v případě přerušení připojení k CRM.
Pokud potřebujete vidět, jak jsem to kdysi řešil, kód naleznete např. tady:
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/Connectivity/Connectivity.fs
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI/blob/master/XElmish/ActorModels.fs

Používal jsem copilota?
Sice ano, ale kód v aplikaci je většinou můj vlastní "pre-LLM" kód recyklovaný z těchto aplikací:
https://github.com/MiroslavHustak/OdisTimetableDownloaderMAUI
https://github.com/MiroslavHustak/Unique_Identifier_And_Metadata_File_Creator
