// 隨機數
const back=document.getElementById('back')
const resert=document.getElementById('resert')
const next=document.getElementById('next')
const countlabel=document.getElementById('countlabel')
let count=0
let random=0

next.onclick=function(){
    random++;
    countlabel.textContent=random;
}

resert.onclick=function(){

random=Math.floor(Math.random()*100)+1;
countlabel.textContent=random;
}

back.onclick=function(){
    random--
    countlabel.textContent=random;
}

// switch
let day=Math.floor(Math.random()*100)+1;
let t;
switch(true)
{
    case day>=50:
        t='>50'
        console.log(t);
        break;
    case day<50:
        t='<50'
        console.log(t);
        break;
        default:
            console.log('nonumber');
}
 

// 字串
let myname='Andy   '
myname=myname.trim().toUpperCase().padEnd(15,'C');
console.log(myname);
 
const email='addd@gmail.com'
let username = email.slice(0,email.indexOf('@'))
let e=email.slice(email.indexOf('@')+1)
console.log(username);
console.log(e);


// while

// let user=''
// do{
//      user=window.prompt('輸入名稱')
// }
// while(user===''||user===null)

// console.log(user);


// 猜數字
// const min=1;
// const max=100;
// const answer=Math.floor(Math.random()*(max-min+1))+min;
// let att=0
// let guess;
// let running=true;

// while(running)
// {
//     guess=window.prompt(`猜一個數字 ${min}到${max}`)
//     guess=Number(guess);

//     if(isNaN(guess))
//     {
//         window.alert('輸入數字')
//     }
//     else if(guess<min||guess>max)
//    {
//      window.alert('輸入範圍內的')
//    }
//    else
//     {
//         att++;
//         if(guess<answer)
//         {
//             window.alert('太小了')
//         }
//         else if(guess>answer)
//         {
//            window.alert('太大了')

//         }
//         else
//         {
//             window.alert('猜對了')
//             running=false;
//         }
//     }
    
// }


// 函式
// let number;
// function isEven(number)
// {
//     return number%2===0?'偶數':'奇數';

// }
// console.log(isEven(5));


// let fruits=['apple','banana','pinapple']



// for(let fruit of fruits)
// {
//     console.log(fruit);
// }


// 解構 ...
// let us = 'aaaa'
// let letters=[...us].join('-')

// console.log(letters);


// function combineStrings(...strings)
// {
//     return strings.join(' ');
// }
// const full=combineStrings('a','b','c')
// console.log(full);


// 製作密碼產生器
function generatePassword(Length,includeLower,includeUpper,includeNumbers,includeSymbols){
    
    const lowerchars='edqasfwfwfwfwefwefwfewf';
    const upperchars='DFSFDSFSDFDSFSDFSDFSDFS';
    const numberchars='0123456789';
    const symbolschars='%$#@!*&)'

    let allowChars='';
    let password='';

    allowChars+=includeLower?lowerchars:'';
  allowChars+=includeUpper?upperchars:'';
  allowChars+=includeNumbers?numberchars:'';
  allowChars+=includeSymbols?symbolschars:'';

  if(Length<=0)
  {
    return `no password`
  }
  if(allowChars.length===0)
  {
    return 'Bad'
  }

  for(let i=0;i<Length;i++)
  {
    const randomIndex=Math.floor(Math.random()*allowChars.length);
    password+=allowChars[randomIndex]
  }

  return password;
}

const passwordLength=10;
const includeLowercase=true;
const includeUppercase=true;
const includeNumbers=true;
const includeSymbols=true;

const password=generatePassword(passwordLength,
                           includeLowercase,
                           includeUppercase,
                           includeNumbers,
                            includeSymbols)

console.log(`${password}`);


// sum(displaypage,1,2)

// function sum(callback,x,y){
//     let result=x+y;
//     callback(result);
// }

// function display(result)
// {
//     console.log(result);
// }


// function displaypage(result)
// {
//     document.getElementById('myH1').textContent=result;
// }

//  回呼 , 陣列內建變數 , 索引 , 陣列本身
//  let numbers=[1,2,3,4,5];

//  numbers.forEach(double)
//  numbers.forEach(display);
//  function display(element)
//  {
//     console.log(element);
//  }

// function double(element,index,array)
// {
//     array[index]=element*2
// }

 
// let fruits=['apple','banana','orange']
// fruits.map(uppercase)
// fruits.map(display)

// function uppercase(element,index,array)
// {
//    array[index]=element.toUpperCase();
// }

// function display(element)
// {
//     console.log(element);
// }

// 轉換格式
// const dates=['2024-1-10','2025-2-20','2026-3-30']
// const formatDates=dates.map(formatDate)
// console.log(formatDates);

// function formatDate(element)
// {  
//    const parts=element.split('-')
//    return `${parts[1]}/${parts[2]}/${parts[0]}`
// }

// accumualator 累加 
// const age=[16,17,18,20,29]
// const adults=age.filter(isAdult)
// console.log(adults);
// function isAdult(element)
// {
//     return element>=18
// }

// const prices=[5,30,10,20,50]
// const total=prices.reduce(sum)
// console.log(`${total.toFixed(0)}`);
// function sum(accumualator,element)
// {
//     return accumualator+element;
// }



// const numbers=[1,2,4,5,6]

// const squers=numbers.map((element)=>Math.pow(element,2))

// console.log(squers);

// 物件
// const person={

//     firstName:"Snoop",
//     lastName:"Dog",
//     age:30,
//     isEmployed:true,
//     sayhello:()=>console.log(`hello,${person.firstName} ${person.lastName}`),
// }

// console.log(person.age);
// person.sayhello();

// person.sayhello();



// constructor
// function Car(make,year){
//    this.make=make,
//    this.year=year,
//    this.drive=function(){console.log(`I drive ${this.make} car`)}
// }

// const car1=new Car('Tasla','2010')

// console.log(car1.make);
// console.log(car1.year);

// car1.drive();


// class類別
// class CarType
// {
//    constructor(name,price)
//    {
//     this.name=name;
//     this.price=price;
//    }

//    displayCount()
//    {
//     console.log(`Product:${this.name}`);
//     console.log(`Price:${this.price}`);
//    }

//    calculates(saleTex){return this.price-(this.price*saleTex)}
// }

// const product1=new CarType('Honda',36000)

// product1.displayCount();

// const sale=0.05;

// const total=product1.calculates(sale);
// console.log(total);


// static 靜態類別
// class User{
   
//     static usercount=0;

//     constructor(username)
//     {
//         this.username=username;
//         User.usercount++;
//     }
   
// }

// const user1=new User('aaa')
// const user2=new User('bbb')

// console.log(user2.username);
// console.log(User.usercount);


// 繼承
// class Animals
// {

//     constructor(name)
//     {
//         this.name=name;
//     }
//     alive=true;
    
//     eat()
//     {
//         console.log(`this${this.name} is eating`);
//     }

//     sleep()
//     {
//         console.log(`this${this.name} is sleeping`);
//     }
// }

// class Rabbit extends Animals
// {
//     constructor(name,year)
//     {
//         super(name)
//         this.year=year
//     }
//       alive=false;
//       name='rabbit';
// }

// class Fish extends Animals
// {
//     name='Fish';
// }

// const rabbit=new Rabbit('rabbi',12);


// console.log(rabbit.alive);
// console.log(rabbit.sleep());
// console.log(rabbit.name);
// console.log(rabbit.year);


// class Animal
// {
//    constructor(weight,height)
//    {
//     this.weight=weight;
//     this.height=height;

//    }

//    set weight(newweight)
//    {
//     if(newweight>0)
//     {
//         this._weight=newweight;
//     }
//     else
//     {
//         console.log('Enter weight');
//     }
//    }

//    set height(newHeight)
//    {
//      if(newHeight>0)
//     {
//         this._height=newHeight;
//     }
//     else
//     {
//         console.log('Enter Height');
//     }
//    }

//    get weight(){
//     return this._weight
//    }
 
//    get height()
//    {
//     return this._height
//    } 
// }

// const ani=new Animal(3,4)

// console.log(ani.height);
// console.log(ani.weight);

// 解構
// function display({name,age=20})
// {
//   console.log(name);
//   console.log(age);
// }


// const person1={
//     name:"Andy",
//     age:30
// }

// const person2={
//     name:"Bob",
  
// }

// display(person1)
// display(person2)


// 嵌套物件
// class Address
// {
//     constructor(street,country)
//     {
//         this.street=street;
//         this.country=country;
//     }
// }

// class Person
// {
//     constructor(name,age,...address)
//     {
//         this.name=name;
//         this.age=age;
//         this.address=new Address(...address)
//     }
// }

// const person1=new Person('andy',20,'大勇街','新北市')

// console.log(person1);


// filter
//  const fruit=[{carlories:100,name:"apple"},{carlories:190,name:"banana"}]

// const low=fruit.filter(f=>f.carlories===100)

// console.log(low);


// reduce
// const max=fruit.reduce((max,fruit)=>fruit.carlories>max.carlories?fruit:max)

// console.log(max.name);

// sort
// fruit.sort((a,b)=>b.carlories-a.carlories)

// console.log(fruit);


// 洗牌遊戲
// const cards=['a',1,2,3,'b',5,6,'c','d']

// shuffle(cards);

// console.log(cards);

// function shuffle(array){
//     for(let i=array.length-1;i>0;i--)
//     {
//       const random=Math.floor(Math.random()*(i+1));

//       [array[i],array[random]]=[array[random],array[i]]
//     }
    
// }

// function createGames()
// {
// let score=0;

// function scoreadd(point){
//    score+=point;
//    console.log(`+${point}`);
// }

// function scoredecrease(point)
// {
//   score-=point;
//   console.log(`-${point}`);
// }
// function getscore()
// {
//   return score;
// }
// return {scoreadd,scoredecrease,getscore}
// }

// const game=createGames()




// game.scoreadd(2);
// game.scoreadd(5);
// game.scoredecrease(4);

// console.log(`得到${game.getscore()}`);

// let timeoutId;

// function starttime()
// {
//    timeoutId=setTimeout(()=>window.alert('Hello'),3000)
//    console.log('start');
// }

// function cleartime()
// {
//    clearTimeout(timeoutId)
//    console.log('clear');
// }



// Json

// const JsonNames =`["Andy","Bob","Cacy"]`
// const JsonPeople=`[{"name":"AAA","age":20},{"name":"BBB","age":30},{"name":"CCC","age":10}]`

// const parseData=JSON.parse(JsonPeople)
// console.log(parseData);



