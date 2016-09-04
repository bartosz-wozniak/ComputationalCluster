# Technology Stack

## Polish version

### Opis

Projekt bazuje na technologii .NET 4.6.1. Planowane jest wykorzystanie następujących języków:

 1. C# (głównie),
 2. F# (w mniejszym stopniu).

Projekt będzie starał się minimalizować zależności między modułami, w zgodzie z ostatnio obowiązującymi trendami. W tym celu wykorzystany będzie wzorzec mediatora i *event bus* w połączeniu z *Command Query Responsibility Separation* (w miejscach, gdzie będzie to miało sens). System będzie wykorzystywał kontener *Dependency Injection* w celu rozdzielenia procesu tworzenia zależności od ich użycia, zwiększając przy okazji niezależność komponentów i rozszerzając wpływ *Inversion of Control* na system.

W celu zapewnienia jakości kodu oraz zapewnienia poprawnego działania systemu, projekt będzie mocno wykorzystywał techniki *test-driven development*, *behaviour-driven development* (jako naturalne rozszerzenie TDD) oraz w niewielkim stopniu testy integracyjne. Pozwoli to zmniejszyć liczbę błędów oraz zapewnić poprawne działanie przy integracji z innymi systemami.

Dla ułatwienia tworzenia projektu, uruchomiony zostanie serwer *continuous integration*, którego celem będzie sprawdzanie, czy dana funkcjonalność działa (uruchamiając testy jednostkowe) i czy w ogóle system się kompiluje.

### Realizacja

W celu osiągnięcia w/w celów, niezbędne jest wykorzystanie zewnętrznych bibliotek, będą to między innymi:

 1. [Autofac](http://autofac.org) jako kontener DI,
 2. [MediatR](https://github.com/jbogard/MediatR) spełniający funkcję mediatora, *event bus* i elementu ułatwiającego implementacje,
 3. [Fixie](http://fixie.github.io) jako framework testów,
 4. [Shouldly](https://github.com/shouldly/shouldly) jako biblioteka asercji,
 5. [FakeItEasy](https://fakeiteasy.github.io) jako biblioteka ułatwiająca tworzenie *mock*ów.

Dodatkowo wykorzystane zostaną następujące biblioteki pomocnicze:

 1. [Paket](https://fsprojects.github.io/Paket) jako zamiennik *NuGet*a,
 2. [FAKE](https://fsharp.github.io/FAKE) jako system automatyzacji budowania projektu.

Wybór powyższych bibliotek był w gruncie rzeczy arbitralny. Dopuszczamy możliwość zmiany tej listy w trakcie prac nad projektem (w zależności od potrzeb).