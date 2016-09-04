# Raport z etapu "komunikacja"

Celem etapu było stworzenie klastra obliczeniowego z częściowo zmokowanymi elementami (TM, CN), którego elementy komunikują się ze sobą, a także potrafią w odpowiedni sposób zareagować na uszkodzenie części systemu.

W trakcie tego etapu mieliśmy do wykonania następujące zadania:

 - Stworzenie szkieletu komunikacji (wspólnej dla każdego komponentu) - 4h
 - Dodanie logów do projektu - 1h
 - Reagowanie na uszkodzenie CS - 3h
 - Procedura startu (załadowanie konfiguracji, połączenie z CS) - 4h
 - Wysyłanie statusu komponentu do CS - 1h
 - Stworzenie CS - 3h
 - Stworzenie BCS, przejęcie przez BCS roli CS - 4h
 - Serializacja wiadomości - 1h
 - Obsługiwanie braku połączenia z klientami (CC, TM, CN) - 3h 
 - Poprawienie systemu rejestracji - 1h
 - Refaktoryzacja kodu - 2h
 - Zarządzanie zadaniami po stronice CS - 4h
 - Stworzenie zmokowanego TM, CN, CC - 1h
 - Przetestowanie systemu - 1h

Niestety nie udało się nam stworzyć działającego systemu na czas (30.03) z powodu problemów z synchronizacją pomiędzy poszczególnymi komponentami.

Cel został osiągniety tydzień później, system został oddany 02.04.
