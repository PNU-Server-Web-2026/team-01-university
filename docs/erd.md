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
        int YearOfStudy "1-6"
        string FieldOfStudy
    }

    Student {
        int Id PK
        string FirstName
        string LastName 
        string Email "Унікальне"
        string RecordBookNumber "Унікальне, 8 цифр"
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
        string Room
        int DayOfWeek "enum: 0-6 (Sunday-Saturday)"
        time BeginTime
        time EndTime
        string WeekType "enum: Both, Odd, Even"
    }
```
*   **Enrollment (Запис на курс):** Логічно це зв'язок багато-до-багатьох, але він змодельований як окрема сутність, оскільки несе власні бізнес-дані: дату запису, оцінку та поточний статус.
*   **Пререквізити (Course - Course):** Факт того, що один курс є пререквізитом для іншого, не має додаткових атрибутів на логічному рівні. Однак для EF Core це самопосилання M:N потребує явної конфігурації проміжної таблиці. У базі даних це буде таблиця CoursePrerequisites із двома зовнішніми ключами: int CourseId та int PrerequisiteId, які посилатимуться на таблицю Course. Згідно з бізнес-правилами, курс НЕ МОЖЕ бути пререквізитом самому собі.
*   **Сурогатні ключі (Id):** Незважаючи на наявність природних унікальних ідентифікаторів, первинними ключами всюди обрано числові `Id` для оптимізації `JOIN` та стабільності ключів.
*   **Політика видалення:** 
    *   При видаленні викладача його курси залишаються без лектора (поле `TeacherId` стає `NULL`, поведінка `SetNull`)[cite: 2].
    *   Видалення кафедри з викладачами або групи зі студентами (Department -> Teacher і StudyGroup -> Student) суворо заборонене на рівні бази даних (поведінка `Restrict`)[cite: 2].