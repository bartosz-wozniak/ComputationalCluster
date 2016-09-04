# Pytania do dokumentacji
1. Brak obsługi błędów (co jeśli CC wyśle prośbę o rozwiązanie nieistniejącego zadania?), 
2. Brak dospecyfikowania sposobu łączenia się z serwerami zapasowymi (co jeśli padnie serwer główny i pierwszy zapasowy? jak jest wykrywane, że CS padł?), 
3. Sposób przesyłania wiadomości jest nieprecyzyjny - jeśli jest jedna wiadomość to kończymy ją '\023' czy nie? Jak kodujemy tego XMLa? UTF-8? 
4. Brak precyzyjnego określenia co się dzieje, gdy CN padnie w trakcie obliczeń - sposób obsługi jest tylko w domyśle, 
5. DVRP - specyfikacja danych zadania? 
6. Doprecyzowania kiedy jest wysyłana "No Operation message". Czy jest wysyłana do każdej odpowiedzi?  
7. Czy dopuszczamy zawodność sieci? Tzn.: Czy dopuszczamy sytuację kiedy oba węzły są sprawne, ale połączenie między nimi nie działa. Jeżeli tak, to w jaki sposób to jest wykrywane?  
8. W wiadomościach nie ma informacji czy obliczenia zakończyły się sukcesem. Możliwe jest, że podane dane są błędne i już na początku obliczeń będzie można zakończyć działanie klastra. Nie dostając informacji od CN czy obliczenia były poprawne serwer zawsze będzie rozsyłał kolejne części zadania do policzenia, zamiast przerwać obliczenia. Z otrzymanych danych CS nie wywnioskuje błędu, bo otrzyma po prostu "Data", nie będzie przecież tego deserializował. 
9. W dokumentacji nie ma diagramów klas, czy mamy dowolność w implementacji?