var helpers=validators.helpers;

function getValidatorParams(){
  
    var rule={type:'regex',regex:"/d/"};
    var params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(1);
    expect(params[0]).to.be("/d/");

    var rule={type:'minLength',minLength:1}
    params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(1);
    expect(params[0]).to.be(1);

    var rule={type:'maxLength',maxLength:3}
    params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(1);
    expect(params[0]).to.be(3);

    var rule={type:'between',from:1,to:5}
    params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(2);
    expect(params[0]).to.be(1);
    expect(params[1]).to.be(5);

    var rule={type:'sameAs',field:"password"}
    params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(1);
    expect(params[0]).to.be("password");

    var rule={type:'required'}
    params=validators.Extend.getValidatorParams(rule);
    expect(params.length).to.be(0);

    var rule={type:'unique'};
    params=validators.Extend.getValidatorParams(rule);
    //todo implemetation
    //expect(params.length).to.be(1);
}

function validateRule(){
  //required
  var rules=[{type:'required'}];
  var value="";
  var result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  value="aa";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);
  //minLength
  var rules=[{type:'minLength',minLength:2}];
  var value="1";
  var result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  var value="22";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  //maxLength
  var rules=[{type:'maxLength',maxLength:2}];
  var value="1";
  var result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  var value="22";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);
  var value="223";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  //between
  var rules=[{type:'between',from:1,to:5}];
  var value=3;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);
  var value=1;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);
  var value=5;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);
  var value=0;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);
  var value=6;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);
//integer
  var rules=[{type:'integer'}];
  var value=3;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  var value="3";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  var value=3.3;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  var value="3s";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);
//Email
  rules=[{type:'email'}];
  var value="3s";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  var value="3s@kooboo.com";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  //ipAddress
  rules=[{type:'ipAddress'}];
  var value="3.2";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  var value="8.8.8.8";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  //numeric
  rules=[{type:'numeric'}];
  var value="aaa";
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);
  var value=3.2;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  var value=3;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  //regex(1-100)

  rules=[{type:'regex',regex:'^([1-9]|[1-9]\\d|100)$'}];
  value=1;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(true);

  value=0;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  value=101;
  result=validators.Extend.validateRule(rules,value);
  expect(result.isValid).to.be(false);

  //todo test sameas
  //todo test unique

}

function resetValidations(){
  var rules=[{type:'required',message:"error"}];
  var validations={
    name:rules
  }
  debugger;
  var newValue=validators.Extend.resetValidations(validations);
  expect(newValue["name"]).not.to.be(undefined);
  var rules=newValue["name"]["rules"];
  expect(rules("")).to.be(false);
  expect(rules("test")).to.be(true);
}

