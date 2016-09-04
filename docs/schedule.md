# Planowanie projektu i podział zadań 

Zgodnie z przyjętą przez nas metodyką Extreme Programming każdy jest odpowiedzialny za cały kod, każdy może dokonywać zmian w całym projekcie. Dzięki temu eliminujemy sytuację, w której każdy odpowiada wyłącznie za jeden moduł i wykonuje prace tylko na nim, nie znając przy tym zupełnie architektury reszty systemu. Zdecydowaliśmy się na takie posunięcie, ponieważ pozwoli nam to na łatwiejszą wymianę zadań w sytuacjach ekstremalnych, typu choroba jednego z członków grupy. Umożliwi to również dokładniejsze testowanie oraz pozwoli na wyczerpujące code review. 

Podsumowując, w praktyce nasz projekt zostanie podzielony na małe zadania, również wewnątrz modułów (typu węzeł obliczeniowy, klaster), którymi podzielimy się tak, aby (w ramach całego projektu) każdy pracował na wszystkich częściach systemu, będziemy również równolegle opiniować fragmenty kodu wykonane przez innych członków z grupy w zupełnie innym module projektu.


## Etapy projektu:
- Komunikacja: 
  - Przesyłanie wiadomości pomiędzy klientem, serwerem i węzłami – etap 1
  - Zabezpieczenie się na wypadek awarii węzła – etap 2
  - Zabezpieczeni się na wypadek awarii serwera – etap 3
  - Przygotowanie serwera zapasowego
    - Oprogramowanie komunikacji serwera zapasowego z węzłami
    - Wymiana informacji pomiędzy serwerami
  - Wyświetlanie komunikatów
- Obliczenia: 
  - Obsługa TS przez TM i CN
    - Ładowanie/wymiana TS przez TM i CN
    - Rozróżnianie TS przez CS
  - Serwer jest w stanie obsługiwać wielu klientów
  - Przygotowanie rozwiązania DVRP 
    - Przygotowanie TS
- Współpraca systemów różnych grup: 
  - Węzły i klient współpracują z serwerami z innych grup
  - Backup server potrafi współpracować z systemami innych grup


## Podział zadań i przewidywany czas wykonania:
- Komunikacja:
  - Przesyłanie wiadomości pomiędzy klientem, serwerem i węzłami (sumarycznie 23h) – etap 1
    - Oprogramowanie wstępnej architektury serwera i komponentów – Jakub, 6h (musi poprzedzać inne, wykonanie przed 13.03)
    - Podstawowa komunikacja CN – CS – Bartosz, 5h, do 16.03
    - Podstawowa komunikacja TM – CS – Rafałm, 5h, do 16.03
    - Podstawowa komunikacja CC – CS – Sławomir, 5h, do 16.03
    - Uzupełnienie komunikatów o wszystkie pozostałe, 2h, do 17.03
  - Zabezpieczenie się na wypadek awarii węzła (sumarycznie 12h) – etap 2
    - Każdy odpowiada za pewien fragment implementacji w razie awarii pewnego węzła, a następnie dochodzi do rotacji zadań
    - Wszyscy oprogramowują różne części serwera 
      - Awaria CN - Bartosz, 4h, do 16.03
      - Awaria TM - Sławomir, 4h, do 16.03
      - Brak połączenia z klientem - Rafał, 4h, do 16.03
  - Zabezpieczeni się na wypadek awarii serwera (sumarycznie 8h) - Jakub, do 16.03 – etap 3
    - Przygotowanie serwera zapasowego
    - Oprogramowanie komunikacji serwera zapasowego z węzłami - etap tożsmy z konfiguracją serwera
    - Wymiana informacji pomiędzy serweram
  - Równolegle:
    - Code review
    - Unit Testy – każdy jest odpowiedzialny za Unit Testy do oprogramowywanej przez siebie części systemu
    - Wyświetlanie komunikatów – każdy jest odpowiedzialny za przygotowanie opcji wyświetlania komunikatów w przygotowanej przez siebie części
- Obliczenia: (dokładny podział zostanie ustalony po pierwszym etapie i zaktualizowaniu informacji, kto dokładnie pracował na jakiej części i w jakim stopniu)
  - Obsługa TS przez TM i CN (sumarycznie 8h)
    - Ładowanie/wymiana TS przez TM i CN
    - Rozróżnianie TS przez CS
  - Serwer jest w stanie obsługiwać wielu klientów, 4h
  - Przygotowanie rozwiązania DVRP, 10h
    - Przygotowanie TS
- Wspołpraca:
  - Poprawki błędów, adaptacja do innych systemów,
  - Wspólna refaktoryzacja kodu