function getVueData(){
  var data=[{
    data:{
      userName:'userName',
    }
  },{
    data:{
      password:'password'
    }
  }];
  var data=Kooboo.Vue.getVueData(data);
  expect(data.data.userName).to.be('userName');
  expect(data.data.password).to.be('password');

}
function getValidationVueData(){
  var data=[{
    data:{
      userName:'userName',
    },
    validations:{
      userName:[{type:'required',message:'required'}]
    }
  },{
    data:{
      password:'password',
    },
    validations:{
      password:[{type:'required',message:'required'}]
    }
  }];

  //username
  var data=Kooboo.Vue.getVueData(data);
  expect(data.validations.userName instanceof Array).not.to.be(true);
  expect(data.validations.userName.rules instanceof Function).to.be(true);
  var rule=data.validations.userName.rules;

  var valid=rule("ss");
  expect(valid).to.be(true);
  valid=rule("");
  expect(valid).to.be(false);

  expect(data.validations.password instanceof Array).not.to.be(true);
  expect(data.validations.password.rules instanceof Function).to.be(true);
  var rule=data.validations.password.rules;

  var valid=rule("ssss");
  expect(valid).to.be(true);
  valid=rule("");
  expect(valid).to.be(false);

} 


function getMergeHooks(){
  var a=0;
  var models=[{
    created:function(){
      a++;
    }
  },{
    created:function(){
      a++;
    }
  }];
  var mergeHooks=Kooboo.Vue.getMergeHooks(models);
  expect(mergeHooks["created"] instanceof Function).to.be(true);

  mergeHooks["created"]();
  expect(a).to.be(2);
}

function parameterBind_getKeyValue(){
  var vue=new Vue();
  var url="aa?id={idx}&data={datax}"
  
  var keyvalue= vue.$parameterBinder().getUrlKeyValue(url);
  expect(Object.keys(keyvalue).length).to.be(3);
  expect(keyvalue["id"]).to.be("idx");
  expect(keyvalue["data"]).to.be("datax");
  expect(keyvalue["siteId"]).to.be("siteId");
}

function parameterBind_bind(){
  var vue=new Vue({
    data:{
      idx:"1",
      datax:"aa"
    }
  });
  var url="aa?id={idx}&data={datax}"
  var url=vue.$parameterBinder().bind(url);
  expect(url).to.be("aa?siteId=&id=1&data=aa");
}