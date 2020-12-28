# MathModelling_SPbSTU
Реализация итерационного решения симплексной таблицы для оптимизционных задач

Механика работы:
1. Запускаем приложение, скачав по ссылке - https://github.com/GeorgGrebenyuk/MathModelling_SPbSTU/raw/main/SimplexMethod1/bin/Debug/SimplexMethod1.exe или из данного репозитория через GitHub Desktop
2. Смотрим на пример импортируемого файла - https://github.com/GeorgGrebenyuk/MathModelling_SPbSTU/blob/main/ExampleData/Test1.txt
Первая строка - коэффициенты при переменных, разделенные ";", наименование функции (max/min)
Строки далее - коэффициенты переменных при уравнениях ограничений. Если какая-либо переменная = 0 (не участвует в уравнении), то ее коэффициент прописывается = 0
3. После создания такого текстового файла запускаем программу и нажатием на "Импорт" выбираем данный файл
4. Нажимаем на "Запуск" для последовательных итераций алгоритма с внесением изменений в видимую таблицу данных до момента, когда не будет найдено оптимальное решение

Примечание: на текущий момент не поддерживается опция минимума целевой функции, все ограничения принимаются <= (без >=)
