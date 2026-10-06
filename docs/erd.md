```mermaid
erDiagram
    Department ||--o{ Teacher : "має (1:N)"
    Teacher |o--o{ Course : "викладає (1:N, SetNull)"
    StudyGroup ||--o{ Student : "включає (1:N)"
    Course ||--o{ Lesson : "включає (1:N)"
    Course }o--o{ Course : "пререквізити (M:N)"
    Student ||--o{ Enrollment : "має записи (1:N)"
    Course ||--o{ Enrollment : "містить записи (1:N)"
    

    Department {
        int Id PK
        string Code "Унікальне, 2-10 великих літер"
        string Name
        string Faculty
    }

    Teacher {
        int Id PK
        string FirstName
        string LastName 
        string Email "Унікальне"
        string Rank "enum: Assistant, SeniorLecturer, AssociateProfessor, Professor"
        int DepartmentId FK
    }

    Course {
        int Id PK
        string Name
        string Code "Унікальне, CS-301"
        int ECTSCredits "1-15"
        int StudentCapacity "1-300"
        int Semester "1-8"
        int TeacherId FK "Може бути null (SetNull)"
    }

    StudyGroup {
        int Id PK
        string Name "Унікальне"
        int StudyCourse "1-6"
        string FieldOfStudy
    }

    Student {
        int Id PK
        string FirstName
        string LastName 
        string Email "Унікальне"
        string NumberOfZalikBook "Унікальне, 8 цифр"
        date Birthday
        int StudyGroupId FK
        string PhotoUrl "Шлях до файлу"
    }

    Enrollment {
        int Id PK
        int StudentId FK "Унікальна пара StudentId + CourseId"
        int CourseId FK "Унікальна пара StudentId + CourseId"
        date Date
        int Mark "0-100, необов'язкове"
        string Status "enum: Active, Completed, Dropped"
    }

    Lesson {
        int Id PK
        int CourseId FK
        string Audience
        string DayOfWeek
        time BeginTime
        time EndTime
        string WeekType "enum: Both, Odd, Even"
    }
```
* **Пререквізити (Course - Course):** Факт того, що один курс є пререквізитом для іншого, не має додаткових атрибутів (оцінок чи дат). Тому на логічній діаграмі це прямий зв'язок M:N без виділення окремої сутності. Проміжна таблиця буде згенерована EF Core неявно.
* **Сурогатні ключі (Id):** Незважаючи на наявність природних унікальних ідентифікаторів (Email, NumberOfZalikBook, Code), первинними ключами всюди обрано числові `Id`. Це конвенція EF Core, що забезпечує швидкість операцій `JOIN` та незмінність ключів.
* **Політика видалення:** При видаленні викладача його курси залишаються без лектора (поле `TeacherId` стає `NULL`, поведінка `SetNull`), щоб не втратити історію курсів.