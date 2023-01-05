grammar HumanTimeGrammar;

input: (date | time | datetime | broadDate) EOF ;
datetime: time' on 'date | date' at 'time | date' from 'time | date' 'time;

date: relativeDate
      | longDateWithoutYear
      | longDateWithYear
      | twoNumbers
      | shortDateWithYear
      | TOMORROW
      | YESTERDAY
      | TODAY
      | NOW
      ;

year: INT;
longDateWithoutYear: month' 'INT SUFFIX? | INT SUFFIX?' '*'of '?month;
longDateWithYear: longDateWithoutYear','?' '*'\''?INT;
twoNumbers: INT'/'' '*INT; // 5/24 could be either May 24th or May 2024
shortDateWithYear: INT'/'' '*INT'/'' '*INT;

offsetDirection: FROM | BEFORE | AFTER;

dateOffsetCount : ('the' | 'a')
                | ((('the'|'a')' '+)INT) 
                | INT;
dateOffsetUnit: DAYS|MONTHS|YEARS;
dateOffset: (dateOffsetCount' '+)?dateOffsetUnit' '+offsetDirection;
relativeDate: dateOffset' '+date;
 
month: JANUARY 
        | FEBRUARY 
        | MARCH 
        | APRIL 
        | MAY 
        | JUNE 
        | JULY 
        | AUGUST 
        | SEPTEMBER 
        | OCTOBER 
        | NOVEMBER 
        | DECEMBER
        ;

broadDate: namedWindow | monthWindow | year | TBD ;
season: SPRING | SUMMER | FALL | HOLIDAY | WINTER;
quarter: Q1 | Q2 | Q3 | Q4;
trimester: EARLY | MID | LATE;
semester: (FIRST_SEMESTER | SECOND_SEMESTER);
namedWindow: (season | quarter | trimester | semester | 'CY')' '*year?;
monthWindow: month' '*year?
            |INT' '+'/'' '+year ; 

hour: INT;
minute: INT;
meridian: AM | PM; 
timeonly: hour' '*O_CLOCK' '*meridian?
    | hour(' '*':'' '*minute)?' '*meridian?
    | MIDNIGHT
    | NOON
    ; 
timezone: TWO_LETTER_TZ | THREE_LETTER_TZ | FOUR_LETTER_TZ;
time: timeonly' '*timezone?;

JANUARY: 'january';
FEBRUARY: 'february';
MARCH: 'march';
APRIL: 'april';
MAY: 'may';
JUNE: 'june';
JULY: 'july';
AUGUST: 'august';
SEPTEMBER: 'september';
OCTOBER: 'october';
NOVEMBER: 'november';
DECEMBER: 'december';
SPRING: 'spring';
SUMMER: 'summer';
FALL: 'fall';
WINTER: 'winter';
HOLIDAY: 'holiday';
Q1: 'q1';
Q2: 'q2';
Q3: 'q3';
Q4: 'q4';
EARLY: 'early';
MID: 'mid';
LATE: 'late';
FIRST_SEMESTER: 'first half of';
SECOND_SEMESTER: 'second half of';
TBD: 'tbd';
INT: [0-9]+;
SUFFIX: 'st' | 'nd' | 'rd' | 'th';
TOMORROW: 'tomorrow';
YESTERDAY: 'yesterday';
DAYS: 'day' | 'days';
MONTHS: 'month' | 'months';
HOURS: 'hour' | 'hours';
MINUTES: 'minute' | 'minutes';
YEARS: 'year' | 'years';
NOW: 'now';
TODAY: 'today';
AFTER: 'after';
BEFORE: 'before';
FROM: 'from';
AM: 'am' | 'a';
PM: 'pm' | 'p';
O_CLOCK: 'o\'clock';
TWO_LETTER_TZ: [a-z][a-z];
THREE_LETTER_TZ: [a-z][a-z][a-z];
FOUR_LETTER_TZ: [a-z][a-z][a-z][a-z];
MIDNIGHT: 'midnight';
NOON: 'noon';
MONDAY: 'monday';
TUESDAY: 'tuesday';
WEDNESDAY: 'wednesday';
THURSDAY: 'thursday';
FRIDAY: 'friday';
SATURDAY: 'saturday';
SUNDAY: 'sunday';
