grammar HumanTimeGrammar;

input: (date | time | datetime) EOF ;
datetime: time' on 'date | date' at 'time | date' from 'time | date' 'time;
date: longDateWithoutYear
      | longDateWithYear
      | shortDateWithoutYear
      | shortDateWithYear
      | relativeDate
      | TOMORROW
      | YESTERDAY
      | NOW
      ;

longDateWithoutYear: month' 'INT SUFFIX| INT SUFFIX' '*'of'?month;
longDateWithYear: longDateWithoutYear','?' '*'\''?INT;
shortDateWithoutYear: INT'/'INT;
shortDateWithYear: INT'/'INT'/'INT;

offsetDirection: FROM | BEFORE | AFTER;

dateOffsetCount : (('the'|'a')' '*INT?) | INT;
dateOffsetUnit: DAYS|MONTHS|YEARS;
dateOffset: dateOffsetCount?' '*dateOffsetUnit' 'offsetDirection' ';
relativeDate: dateOffset date;
 
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

hour: INT;
minute: INT;
meridian: AM | PM; 
timeonly: hour' '*O_CLOCK
    | hour(' '*':'' '*minute)?' '*meridian
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
