# ViajaNet.ThiagoMancuzo.Metrics

O projeto consiste em 3 consoles, sendo:
 - ViajaNet.ThiagoMancuzo.Metrics.Reader.App:
   - Obtém métricas de um determinado site utilizando o Google Analytics Reports, enviando as informações para um bus que implementa RabbitMQ;
- ViajaNet.ThiagoMancuzo.Metrics.Receiver.App:
  - Recebe as mensagens pelo RabbitMQ, e às persiste no Couchbase
- ViajaNet.ThiagoMancuzo.Metrics.Viewer.App:
  - Realiza uma consulta utilizando Map/Reduce para exibir médias de sessões por cidade
 
  
Para execução do projeto:
 - Inserir o arquivo de credenciais do Google API na pasta ViajaNet.ThiagoMancuzo.Metrics.Reader.App;
 - Criar uma view com design = sessions / name = by_city, com o Map/Reduce abaixo:
 
### Map:
```javascript
function (doc, meta) {
  if(doc.type == 'SessionMetrics') {
  	emit(doc.city,doc.sessionCount);
  }
}
```

### Reduce:
```javascript
function (keys, values, rereduce)
{
  if(!keys) 
    return null;
  
  var out = []; 
  var lastKey = keys[0];
  var avg = 0;
  
  for (var i in keys) {
    if(lastKey != keys[i]) {
      out.push({city: lastKey, avg:  avg/keys.length});
      lastKey = keys[i];
      avg = 0;
    }
    
    avg = avg + values[i];
  }
  
  if(keys) {
    out.push({city: lastKey, avg: avg/keys.length});
  }
  
  return out;
}
```
