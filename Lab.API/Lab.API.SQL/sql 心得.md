# Sql 心得

### 1. 基本查詢
### 觀念查詢 => https://realnewbie.com/posts/tags/sql-database-101
### 測驗網址 => https://sqlzoo.net/wiki/SQL_Tutorial

1. 查詢面積為 5,000,000 以上平方公里的國家,對每個國家顯示她的名字和人均國內生產總值(gdp/population)

```sql
SELECT name, gdp/population 
FROM world
WHERE area > 5000000
```

2. 檢查列表:單詞“IN”可以讓我們檢查一個項目是否在列表中 , 顯示“Ireland 愛爾蘭”,“Iceland 冰島”,“Denmark 丹麥”的國家名稱和人口 

```sql
SELECT name, population 
FROM world
WHERE name IN ('Ireland', 'Iceland ', 'Denmark');
```

3. 顯示面積為 200,000 及 250,000 之間的國家名稱和該國面積

```sql
SELECT name, area 
FROM world
WHERE area BETWEEN 200000 AND 250000
```

4. 找出有至少200百萬(2億)人口的國家名稱，及人均國內生產總值 , (人均國內生產總值，即是國內生產總值除以人口(GDP/population))
```sql
Select name,gdp/population
from world
where population>200000000
```

5. 顯示'South America'南美洲大陸的國家名字和以百萬為單位人口數

```sql
select name,population /1000000
from world
where continent='South America'
```

6. 顯示包含單詞“United”為名稱的國家
```sql
Select name
from world 
where name like '%United%'
```

7. 美國、印度和中國(USA, India, China)是人口又大，同時面積又大的國家。排除這些國家。顯示以人口或面積為大國的國家，但不能同時兩者。顯示國家名稱，人口和面積

```sql
Select name,population,area
from world
where (area>3000000 or population>250000000) 
and not (area>3000000 and population>250000000)
```

9. 除以為1000000（6個零）是以百萬計。除以1000000000（9個零）是以十億計。使用 ROUND 函數來顯示的數值到小數點後兩位。
對於南美顯示以百萬計人口，以十億計2位小數GDP

```sql
SELECT 
    name, 
    ROUND(population/1000000, 2), 
    ROUND(gdp/1000000000, 2)
FROM world
WHERE continent = 'South America'
```

10. 顯示萬億元國家的人均國內生產總值，四捨五入到最近的$ 1000

```sql
Select name,round(gdp/population,-3)
from world
where gdp>1000000000000
```

11. 顯示以 N 開頭的國家的名稱，但將大洋洲替換為澳大拉西亞

```sql
SELECT name, 
       CASE WHEN continent='Australasia' THEN 'North Oceania'
            ELSE continent END
  FROM world
 WHERE name LIKE 'N%'
```
12. 顯示名稱和所在洲——但將“歐洲”和“亞洲”替換為 “歐亞大陸 ”；將 “北美洲” 中的每個國家替換為 “美洲 ”。 或南美洲或加勒比海地區 。顯示以字母 A 或 B 開頭的國家

```sql
Select name,
case 
   when continent in ('Europe','Asia') then 'Eurasia'
   when continent in ('North America','South America','Caribbean') then 'America'
   else continent end
from world
where name like 'A%' or name like 'B%'
```

10. 顯示萬億元國家的人均國內生產總值，四捨五入到最近的$ 1000

```sql
Select name,round(gdp/population,-3)
from world
where gdp>1000000000000
```

### 子查詢

1. 列出每個國家的名字 name，當中人口 population 是高於俄羅斯'Russia'的人口

```sql
SELECT name FROM world
  WHERE population >
     (SELECT population FROM world
      WHERE name='Russia')
```

2. 在阿根廷Argentina 及 澳大利亞 Australia所在的洲份中，列出當中的國家名字 name 及洲分 continent 。按國字名字順序排序

```sql
Select name,continent 
from world
where continent in (select continent from world where name in ('Argentina','Australia'))
```

3. Germany德國（人口8000萬），在Europe歐洲國家的人口最多。Austria奧地利（人口850萬）擁有德國總人口的11％。
顯示歐洲的國家名稱name和每個國家的人口population。以德國的人口的百分比作人口顯示

```sql
select name,concat(round(100.0*population/(select population from world where name='Germany'),0),'%')
from world
where continent='Europe'
```

4. 我們可以用ALL 這個詞對一個列表進行>=或>或<或<=充當比較。例如，你可以用此查詢找到世界上最大的國家(以人口計算)

```sql
SELECT name
  FROM world
 WHERE population >= ALL(SELECT population
                           FROM world
                          WHERE population>0)
```

5. 在每一個州中找出最大面積的國家，列出洲份 continent, 國家名字 name 及面積 area。 (有些國家的記錄中，AREA是NULL，沒有填入資料的。)

```sql
SELECT continent, name, area FROM world x
  WHERE area>= ALL
    (SELECT area FROM world y
        WHERE y.continent=x.continent
          AND area>0)
```

6. 列出洲份名稱，和每個洲份中國家名字按子母順序是排首位的國家名。(即每洲只有列一國)

```sql
Select continent,name
from world x
where name<=all(select name from world y where y.continent=x.continent)
```

7. 有些國家的人口是同洲份的所有其他國的3倍或以上。列出 國家名字name 和 洲份 continent

```sql
select name,continent
from world x
where (population/1000)>=All(select (population/1000)*3 from world y where y.continent=x.continent and y.name!=x.name)
```

### Join

1. 列出 賽事編號matchid 和球員名 player ,該球員代表德國隊Germany入球的。要找出德國隊球員，要檢查: teamid = 'GER'

```sql
SELECT matchid,player 
from goal g
join eteam e
on g.teamid=e.id
where teamid='GER'
```

2. 只列出全部賽事，射入德國龍門的球員名字

```sql
SELECT distinct player
  FROM game JOIN goal ON matchid = id 
    WHERE (team1='GER' or team2='GER') and teamid!='GER' 
```

3. 列出隊伍名稱 teamname 和該隊入球總數

```sql
SELECT teamname, count(gtime)
  FROM eteam JOIN goal ON id=teamid
group by teamname
ORDER BY teamname
```

4. 每一場波蘭'POL'有參與的賽事中，列出賽事編號 matchid, 日期date 和入球數字

```sql
SELECT matchid,mdate,count(gtime)
  FROM game JOIN goal ON matchid = id 
 WHERE (team1 = 'POL' OR team2 = 'POL')
group by matchid,mdate
```

5. 每一場德國'GER'有參與的賽事中，列出賽事編號 matchid, 日期date 和德國的入球數字

```sql
SELECT matchid,mdate,count(gtime)
  FROM game JOIN goal ON matchid = id 
 WHERE (team1 = 'GER' OR team2 = 'GER') and teamid='GER'
group by matchid,mdate
```

6. 查詢中列出了所有進球。如果進球者是 team1，則 score1 列的值會顯示為 1，否則為 0。您可以對該列求和，得到 team1 的進球數。然後 ，按 mdate、matchid、team1 和 team2 對結果進行排序

```sql
SELECT mdate,
       team1,
       SUM(CASE WHEN teamid = team1 THEN 1 ELSE 0 END) AS score1,
       team2,
       SUM(CASE WHEN teamid = team2 THEN 1 ELSE 0 END) AS score2
FROM game LEFT JOIN goal ON id = matchid
GROUP BY mdate, matchid, team1, team2
ORDER BY mdate, matchid, team1, team2
```

7. 尊·特拉華達'John Travolta'最忙是哪一年? 顯示年份和該年的電影數目

```sql
SELECT yr,COUNT(title) FROM
  movie JOIN casting ON movie.id=movieid
         JOIN actor   ON actorid=actor.id
where name='John Travolta'
GROUP BY yr
HAVING COUNT(title)=(SELECT MAX(c) FROM
(SELECT yr,COUNT(title) AS c FROM
   movie JOIN casting ON movie.id=movieid
         JOIN actor   ON actorid=actor.id
 where name='John Travolta'
 GROUP BY yr) AS t
)
```

8. 列出演員茱莉·安德絲 'Julie Andrews' 曾參與的電影名稱及其第1主角。 是否列了電影 "Little Miss Marker" 兩次 ? 她於1980再參與此電影Little Miss Marker. 原作於1934年,她也有參與。 電影名稱不是獨一的。在子查詢中使用電影編號

```sql
SELECT title,name
FROM casting c
join movie m on m.id=c.movieid
join actor a on a.id=c.actorid
where ord=1 and movieid in 
(select movieid 
 from casting c 
 join actor a on a.id=c.actorid where a.name='Julie Andrews')

```

9. 列出1978年首影的電影名稱及角色數目，按此數目由多至少排列
```sql
select m.title,count(a.name)
from movie m
join casting c on c.movieid=m.id
join actor a on c.actorid=a.id
where m.yr=1978
group by m.title
order by count(a.name) desc
```

10. 列出曾與演員亞特·葛芬柯'Art Garfunkel'合作過的演員姓名

```sql 
SELECT distinct name
FROM actor
JOIN casting ON actor.id = casting.actorid
WHERE movieid IN (
  SELECT movieid 
  FROM casting 
  JOIN actor ON actor.id = casting.actorid 
  WHERE name = 'Art Garfunkel'
) 
AND name != 'Art Garfunkel';
```

### Inner , Left , Right Join

1. Inner Join 在 join 到其中一個有 null 欄位時會直接跳過

```sql
SELECT teacher.name, dept.name
 FROM teacher 
 INNER JOIN dept ON (teacher.dept=dept.id)
```

2. 使用不同的JOIN(外連接)，來列出全部老師 => left join 注重左邊 , 所以右邊 null 也會加進來

```sql
select t.name,d.name
from teacher t
left join dept d on d.id=t.dept
```

3. coalesce 會照順序取括號裡的欄位值 , 如果第一個是空的就往下一個取 , 以此類推直到取到一個

```sql
select name,coalesce(mobile,'07986 444 2266')
from teacher
```

4. 使用 CASE 函數顯示每位教師的姓名，如果該教師在系 1 或系 2，則顯示“Sci”；如果該教師在系 3，則顯示“Art”；否則顯示“None”

```sql
select name,
case
   when dept=1 or dept=2 then 'Sci'
   when dept=3 then 'Art'
   else 'None'
end
from teacher
```

### SelfJoin 自己 Join 自己

1. 以下查詢列出途經 London Road (149) 或 Craiglockhart (53)的巴士線號碼。注意有兩條路線會經過這兩個站兩次。 加入 HAVING 語句來限制只列出這兩條路線

```sql
SELECT company, num, COUNT(*)
FROM route WHERE stop=149 OR stop=53
GROUP BY company, num
having count(*)=2
```

2. 執行自我合拼來，留意b.stop代表由Craiglockhart出發不用轉車可前住的地方。 修改它來顯示由Craiglockhart到 London Road的服務資料

```sql
SELECT a.company, a.num, a.stop, b.stop
FROM route a JOIN route b ON
  (a.company=b.company AND a.num=b.num)
WHERE a.stop=53 and b.stop=149
```

3. 此題和上題相似，但是用兩個stops表來自我合拼。這樣我們可以用站名而非站編號。 修改它來顯示由Craiglockhart到 London Road的服務資料

```sql
SELECT a.company, a.num, stopa.name, stopb.name
FROM route a JOIN route b ON
  (a.company=b.company AND a.num=b.num)
  JOIN stops stopa ON (a.stop=stopa.id)
  JOIN stops stopb ON (b.stop=stopb.id)
WHERE stopa.name='Craiglockhart' and stopb.name='London Road'
```

4. SelfJoin=>列出連接115 和 137 ('Haymarket' 和 'Leith') 的公司名和路線號碼。不要重覆

```sql
select distinct a.company,a.num
from route a 
join route b on (a.company=b.company and a.num=b.num)
join stops stopa on (a.stop=stopa.id)
join stops stopb on (b.stop=stopb.id)
where stopa.name='Haymarket' and stopb.name='Leith'
```

5. 不重覆列出可以由 'Craiglockhart' 乘一程車到達的站stops，包括'Craiglockhart'本身。 列出站名，公司名和路線號碼

```sql
select distinct stopb.name,b.company,b.num
from route a
join route b on (a.company=b.company and a.num=b.num)
join stops stopa on a.stop=stopa.id
join stops stopb on b.stop=stopb.id
where stopa.name='Craiglockhart'
```

6. Join 兩個路線 , r1 => r2 代表起點站到轉車站 (Craiglockhart), r3 => r4 代表轉車站到終點站 (Sighthill)

```sql
select distinct r1.num,r2.company,middle.name,r3.num,r3.company
from route r1
join route r2 on (r1.num=r2.num and r1.company=r2.company)
join route r3 on (r2.stop=r3.stop)
join route r4 on (r3.num=r4.num and r3.company=r4.company)
join stops startstop on (r1.stop=startstop.id)
join stops middle on r3.stop=middle.id
join stops endstop on (r4.stop=endstop.id)
where startstop.name='Craiglockhart' and endstop.name='Sighthill'
```