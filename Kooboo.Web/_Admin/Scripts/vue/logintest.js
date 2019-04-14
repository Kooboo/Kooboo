var vm=new Vue({
    el:'#container',
    data:{
       UserName:{
          value:"",
          validate:{
             rules:[
                {type:"required",message:"username can't be empty"},
                {type:"maxLength",message:"username max Length is 50"},
             ]
          },
          isValid:true,
          errors: []
       },
       Password:{
          value:"",
          validate:{
             rules:[
                {type:"required",message:"password can't be empty"},
             ]
          },
          isValid:true,
          errors: []
       },
       Remember:{
          value:"",
       },
       Returnurl:{
          value:"",
       },
    },
    methods:{
       onLogin:function(){
          function isValid(){
             this.$data.UserName.isValid&&this.$data.Password.isValid
          }
          if(isValid()){return ;}
          Kooboo.User.Login({
             UserName:this.$data.UserName.value,
             Password:this.$data.Password.value,
             Remember:this.$data.Remember.value,
             Returnurl:this.$data.Returnurl.value,
          }).then(function(res){
             redirect(res);
          })
       }
    },
 })
 