function VariableDataManager(){
    var bindResult={};
    var param={
        model:"EditHtml",
        converterKoobooId:null,
        currentVariables:[],
    }
    function getMergedVariables(){
        var allVariables=[];
        if(bindResult[param.converterKoobooId]){
            var result= bindResult[param.converterKoobooId];
            allVariables=allVariables.concat(result);
        }
        allVariables=allVariables.concat(param.currentVariables);

        var mergedVariables={};
        for(var i=0;i<allVariables.length;i++){
            var variable=allVariables[i]
                koobooId=variable.koobooId,
                mergedVariables[koobooId]=variable;
        }
        return mergedVariables;
    }
    return {
        getModel:function(){
            return param.model
        },
        setModel:function(model){
            param.model=model;
        },
        setConverterId:function(converterKoobooId){
            param.converterKoobooId=converterKoobooId;
        },
        add:function(variableKoobooId,variableName){
            var variable = {};
            variable["koobooId"] = variableKoobooId;
            variable["name"] = variableName;
            param.currentVariables.push(variable);
        },
        getVariableName:function(variableKoobooId){
            var mergedVariables=getMergedVariables();
            var variable=mergedVariables[variableKoobooId];
            if(variable){
                return variable.name;
            }
            return "";
        },
        clearCurrentVariables:function(){
            param.currentVariables=[];
            
        },
        clear:function(){
            if(bindResult[param.converterKoobooId]){
                delete bindResult[param.converterKoobooId];
            }
        },
        getBindResult:function(){
            var mergedVariables=getMergedVariables();
            param.currentVariables=[];
            if(!bindResult[param.converterKoobooId]){
                bindResult[param.converterKoobooId]=[];
            }
            $.each(mergedVariables,function(i,variable){
                bindResult[param.converterKoobooId].push(variable);
            });
            return bindResult[param.converterKoobooId];
        }
    }
}