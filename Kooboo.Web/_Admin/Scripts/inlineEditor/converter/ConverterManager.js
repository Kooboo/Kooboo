function ConverterManager(){
    return {
        getConverters: function() {
            var converts = [];
            converts.push(Kooboo.converterTypes.HtmlBlock);
            converts.push(Kooboo.converterTypes.View);
            converts.push(Kooboo.converterTypes.ContentList);
            //converts.push(Kooboo.converter.Category);
            converts.push(Kooboo.converterTypes.Menu);
            return converts;
        },
        getConverterByName: function(name) {
            var converters = this.getConverters();
            for (var i = 0; i < converters.length; i++) {
                var converter = converters[i];
                if (name && converter.name.toLowerCase() == name.toLowerCase()) {
                    return converter;
                }
            }
            return null;

        },
        executeConverter: function(name, selectedElement) {
            var converter = this.getConverterByName(name);
            if (converter) {
                return converter.execute(selectedElement);
            }
            return null;
        },
        getConverterResultByElement: function(el) {
            var converters = this.getConverters();
            var convertResult = {},
                self=this;
            $.each(converters, function(i, converter) {
                var converterName = converter.name;
                var result = self.executeConverter(converterName, el);
                if (result.hasResult) {
                    convertResult[converterName] = result;
                }
            });
            return convertResult;
        },
        isBeConvertedElement: function(convertElements, element) {
            var convertedElement = false;
            $.each(convertElements, function(i, convertElement) {
                convertedElement = convertElement.el == element;
                if (convertedElement)
                    return true;
            });
            return convertedElement;
        },
        isInConvertedElement: function(convertElements, element) {
            var inConvertedElement = false;
            $.each(convertElements, function(i, convertElement) {
                inConvertedElement = $(convertElement.el).find(element).length > 0;
                if (inConvertedElement)
                    return true;
            });
            return inConvertedElement;
        },
        isContainConvertedElement: function(convertElements, element) {
            var isContain = false;
            $.each(convertElements, function(i, convertElement) {
                isContain = $(element).find(convertElement.el).length > 0;
                if (isContain)
                    return true;
            });
            return isContain;
        },
    }
}