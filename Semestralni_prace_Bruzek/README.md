# Semestralka_Bruzek
Jednoduchá aplikace na vytvoření faktury.
Jako první nainstalujeme System.Data.SQLite.
(Ještě to po mě před buildem na druhém PC chtělo instalovat .NET 8.0 Desktop Runtime(dlouho na něm VS neběželo, případně nutno doinstalovat, je to malá instalačka a nabídne to samo visualStudio)).
Pro vytvoření faktury je třeba si založit vlastní firmu a bankovní účet.
Při tvorbě faktury se musí přidat odběratel, položky, vybrat bankovní účet a poté je možno fakturu uložit.
Odběratel se přidá přes tlačítko a edituje se přes téže tlačítko(mění se text).
Položka se vytvoří přes tlačítko a edituje se přes dvojklik na položku. U položky se liší, zda jste plátce DPH či ne(když je neplátce, DPH je vždy 0%). Odstranit se dá přes dvojklik na položku a tlačítko odstranit
Fakturu je možné v seznamu faktur editovat.