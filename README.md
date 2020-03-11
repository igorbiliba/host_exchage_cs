Host exchange
=====================
- При каждом запуске генерит новый ssl сертификат и добавляет в доверенные в винде
- Запускается от администратора
- http авторизация в настройках rpc_login и rpc_passwrd
- для заливки релиза и обновления тикет клиентов "php push.php"

### Релизы
https://github.com/igorbiliba/host_exchage_cs/releases/

### Зависимости
- https://github.com/igorbiliba/host_exchage_cs
- https://github.com/igorbiliba/netex_client_cs
- https://github.com/igorbiliba/365cash_client_cs
- https://github.com/igorbiliba/mine_exchange_cs
- https://github.com/igorbiliba/konvert_cs

Init
=====================
### Settings.json
- rate_path - "on_client", чтобы просить рэйт у клиента, ну или указать путь из bestchange "63;93;521"
- storage_btc_addr_types - типы адресов, что генерить заранее
- storage_btc_addr_cnt - кол-во адресов сгенерированных заранее для каждого типа
- max_repeat_on_fault - кол-во неудачных попыток для одного клиента
- canceled_min - через столько применится status "canceled" если на сгенерированный адрес btc не придет бабло
- btc_addresstype - What type of addresses to use ("legacy", "p2sh-segwit", or "bech32", default: "p2sh-segwit"). Пустой, чтобы оставить настройку btc клиента. get_from_ticket_client - чтобы спросить тип адреса у тикет клиента.
- use_ssl_and_rpc_auth - Http авторизация и использование https.
- allow_methods - методы, что разрешены для rpc, *- разрешить все.
- makecert_path - только для https режима.
- demping_percent - занизить курс на n%

Тестовый режим
=====================
### Создать файл (Test.json) рядом с .exe
- Эмуляция данных ответа тикета, btc адреса, btc баланса
- Для боевого режима удлить файл и перезагрузить программу
```js
{
  "ticket": { "account": "+79060671232", "comment": "#3877525#", "btc_amount": "0.8756" },
  "btc_income": "0.8756",
  "btc_address": "3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG",
  "fake_send_btc": "3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG"
}
```
- ticket - якобы созданный тикет от нетекса.
- btc_income - якобы входящее бабло от btc клиента.
- btc_address - якобы только что сгенерированный адрес от btc клиента.
- fake_send_btc - будет всегда якобы отправлять бабло на этот btc адрес.

API
=====================
### Создание заявки
https://localhost:8086/create?amount=6000&phone=79060671232
##### либо
https://localhost:8086/create?amount=6000&phone=79060671232&forwardtobtc=3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG&dempingpercent=5
- использовать либо dempingpercent, либо (forwardint, forwardfraction)
- forwardtobtc - адрес, куда будет отправлен перевод, после подтверждения статуса заявки. (не обязательное поле)
- dempingpercent - процент, на который понизится значение при перебросе на forwardtobtc. (не обязательное поле)
##### либо
https://localhost:8086/create?amount=6000&phone=79060671232&forwardtobtc=3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG&forwardint=0&forwardfraction=002
- использовать либо dempingpercent, либо (forwardint, forwardfraction)
- forwardtobtc - адрес, куда будет отправлен перевод, после подтверждения статуса заявки. (не обязательное поле)
- forwardint {int}, forwardfraction {char(8)} - сумма, которая должна перебросится клиенту на forwardtobtc. forwardint - целое число, forwardfraction - число после запятой.
#### ответ:
```js
{
    "hash":"3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG",
    "account":"+79265530503",
    "comment":"#4043198#",
    "btc_amount":"0,00865211730523684",
    "client":[
        {
            "Key":"rate",
            "Value":"691956,858836499"
        },
        {
            "Key":"balance",
            "Value":"8,81967163085938"
        },
        {
            "Key":"name",
            "Value":"Lisa"
        }
    ]
}
```
- btc_amount - реальная сумма, без процента демпинга
### Статус заявки
https://localhost:8086/check?hash=5dba462b9ab77ac5dc158eb5047367f0
#### ответ:
```js
{
  status : "paid"
}
```
```js
{
  status : "canceled"
}
```
```js
{
  status : "pending"
}
```
### Курс
https://localhost:8086/cource
#### ответ:
```js
{
  cource : "595775.6851",
  balance : "3.81",
  client : "Bart"
}
```
https://localhost:8086/cources
#### ответ:
```js
[
  {
    cource : "595775.6851",
    balance : "3.81",
    client : "Bart"
  },
  {
    cource : "594775.6851",
    balance : "5.31",
    client : "Lisa"
  }
]
```
- Вернет наименьший курс средии клиентов.
- client - покажет какой клиент использует такой курс.
### Валидация btc адреса
https://localhost:8086/btc_validate?address=3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG
- не работает в тестовом режиме, то есть не использовать без btc клиента
#### ответ:
```js
{
  isvalid : true,
  adress : "3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG",
}
```

Прочее
=====================
### Спарсить btc адреса
1. Открыть https://www.blockchain.com/btc/address/1F1tAaz5x1HUXrCNLbtMDqcw6o5GNn4xqX
2. Вставить в консоль содержимое https://code.jquery.com/jquery-3.4.1.min.js
3. выполнить
```js
var list = [];
$("a").each(function(key, el) {
	if($(el).attr("href").indexOf("/btc/address/") === 0)
    list.push($(el).html());
});
console.log(list);
```
