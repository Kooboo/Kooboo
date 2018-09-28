/*******************************************************************************
 *
 *  Copyright © 2011-2014 Hewlett-Packard Development Company, L.P. 
 *
 *  This work is distributed under the W3C® Software License [1] 
 *  in the hope that it will be useful, but WITHOUT ANY 
 *  WARRANTY; without even the implied warranty of 
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 *
 *  [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231 
 *
 *  Adapted from the Mobile Test Harness
 *  Copyright © 2007 World Wide Web Consortium
 *  http://dev.w3.org/cvsweb/2007/mobile-test-harness/
 * 
 ******************************************************************************/

/**
  Data returned from server:
  
  response.title;
  response.description;
  response.heading; *
  response.build_date;
  response.lock_date; *
  response.test_uri;
  response.results_uri;
  response.details_uri;
  response.client_engine;
  response.is_index; *
  response.engine_titles = { _engineName: title, ... };
  response.sections = { _anchor: section_data, ... };
  
  section_data.anchor_name; *
  section_data.section_name; *
  section_data.test_count;
  section_data.results = { _engineName: result_data, ... }
 
  result_data.pass_count;
  result_data.fail_count;

**/

var annotator = {
  QUERY_URI:          "https://test.csswg.org/harness/api/status/",
  STYLESHEET_URI:     "https://test.csswg.org/harness/stylesheets/annotate.css",
  NEED_TEST_ICON_URI: "https://test.csswg.org/harness/img/please_help_12.png",
  ENGINE_LOGOS: { '_gecko': "https://test.csswg.org/harness/img/gecko.svg",
                  '_presto': "https://test.csswg.org/harness/img/presto.svg",
                  '_trident': "https://test.csswg.org/harness/img/trident.svg",
                  '_webkit': "https://test.csswg.org/harness/img/webkit.svg" },

  mResponse: null,
  mClosed: false,
  
  buildURI: function(base, sectionName) {
    if (sectionName) {
      return base + 'section/' + sectionName + '/';
    }
    return base;
  },
  
  removeAnnotation: function(anchorName) {
    try {
      var annotation = document.getElementById('annotation_' + (anchorName ? anchorName : 'root_'));

      if (annotation) {
        annotation.parentNode.removeChild(annotation);
      }
    }
    catch (err) {
    }
  },
  
  removeAllAnnotations: function () {
    try {
      if (this.mResponse && this.mResponse.sections) {
        for (index in this.mResponse.sections) {
          if (this.mResponse.sections.hasOwnProperty(index)) {
            this.removeAnnotation(this.mResponse.sections[index].anchor_name);
          }
        }
      }
    }
    catch (err) {
    }
  },
  
  toggleAnnotations: function() {
    this.mClosed = (! this.mClosed);
    this.removeAllAnnotations();
    this.addAnnotations();
  },
  
  toggleDetails: function(domEvent) {
    var engineNode = domEvent.target;
    while (engineNode && ('SPAN' != engineNode.tagName.toUpperCase())) {
      engineNode = engineNode.parentNode;
    }
    var engineName = engineNode.getAttribute('data-engineName');
    var annotation = engineNode.parentNode.parentNode;
    var details = annotation.lastChild;
    
    if (engineName == details.getAttribute('data-engineName')) {
      details.setAttribute('class', 'details');
      details.removeAttribute('data-engineName');
    }
    else {
      details.setAttribute('data-engineName', engineName);
      
      var engineTitle = this.mResponse.engine_titles[engineName];
      var section = annotation.getAttribute('data-section');

      var detailsEngine = details.firstChild;
      var detailsLink = detailsEngine.lastChild;
     
      detailsEngine.firstChild.textContent = engineTitle + ' ';
      detailsLink.setAttribute('href', this.buildURI(this.mResponse.results_uri, section));
      
      var meter = details.lastChild;
      var numbers = meter.firstChild;
      var passBar = numbers.nextSibling;
      var failBar = passBar.nextSibling;
      var needBar = failBar.nextSibling;
      numbers.textContent = engineNode.getAttribute('title');
      
      var passCount = parseInt(engineNode.getAttribute('data-passCount'), 10);
      var failCount = parseInt(engineNode.getAttribute('data-failCount'), 10);
      var needCount = parseInt(engineNode.getAttribute('data-needCount'), 10);
      var total = passCount + failCount + needCount;

      passBar.setAttribute('style', 'width: ' + ((passCount / total) * 100.0) + '%');
      failBar.setAttribute('style', 'width: ' + ((failCount / total) * 100.0) + '%');
      needBar.setAttribute('style', 'width: ' + ((needCount / total) * 100.0) + '%');
      
      details.setAttribute('class', 'details open');
    }
    
  },
  
  addAnnotationTo: function(anchorElement, section, first) {
    try {
      var headings = {'h1':'', 'h2':'', 'h3':'', 'h4':'', 'h5':'', 'h6':'',
                      'H1':'', 'H2':'', 'H3':'', 'H4':'', 'H5':'', 'H6':''};
      var targetElement = anchorElement;
      
      while (targetElement && (Node.ELEMENT_NODE == targetElement.nodeType) && (! (targetElement.tagName in headings))) {
        targetElement = targetElement.parentNode;
      }
      if (targetElement && (Node.ELEMENT_NODE == targetElement.nodeType)) {
        var needCount = section.test_count;
        for (engineName in section.results) {
          if (section.results.hasOwnProperty(engineName)) {
            var engineResults = section.results[engineName];
            if (this.mResponse.engine_titles[engineName] == this.mResponse.client_engine) {
              needCount = section.test_count - (engineResults.pass_count + engineResults.fail_count);
              break;
            }
          }
        }

        var annotation = document.createElement('div');

        annotation.setAttribute('id', 'annotation_' + ((! section.anchor_name) ? 'root_' : section.anchor_name));
        var annotationClass = 'annotation';
        if (first) {
          annotationClass += ' first';
        }
        if (0 < needCount) {
          annotationClass += ' need';
        }
        if (this.mClosed) {
          annotationClass += ' closed';
        }
        annotation.setAttribute('class', annotationClass);
        if (section.section_name) {
          annotation.setAttribute('data-section', section.section_name);
        }
        annotation.setAttribute('data-testCount', section.test_count);
        annotation.setAttribute('data-needCount', needCount);

        // disclosure control
        if (first) {
          var disclosure = document.createElement('div');
          disclosure.setAttribute('class', 'disclosure button');
          disclosure.setAttribute('onclick', 'annotator.toggleAnnotations()');
          annotation.appendChild(disclosure);
        }
        
        // close box
        var closeBox = document.createElement('div');
        closeBox.setAttribute('class', 'close button');
        if (first) {
          closeBox.setAttribute('onclick', 'annotator.removeAllAnnotations()');
        }
        else {
          closeBox.setAttribute('onclick', 'annotator.removeAnnotation("' + section.anchor_name + '")');
        }
        annotation.appendChild(closeBox);
        
        // Test suite info
        if ((! this.mClosed) && first && this.mResponse.is_index && this.mResponse.heading) {
          var title = document.createElement('div');
          title.setAttribute('class', 'title');
          
          title.innerHTML = this.mResponse.heading;
          
          annotation.appendChild(title);
        }
        
        // Test count heading
        var heading = document.createElement('div');
        heading.setAttribute('class', 'heading');
        
        var testLink = document.createElement('a');
        testLink.setAttribute('href', this.buildURI(this.mResponse.test_uri, section.section_name));

        if (0 == section.test_count) {
          testLink.appendChild(document.createTextNode('No Tests'));
        }
        else if (1 == section.test_count) {
          testLink.appendChild(document.createTextNode('1 Test'));
        }
        else {
          testLink.appendChild(document.createTextNode(section.test_count + ' Tests'));
        }
        
        // Testing needed text
        if ((! this.mClosed) && (! this.mResponse.lock_date) && (0 < needCount)) {
          var untested = document.createElement('span');
          var image = document.createElement('img');
          image.setAttribute('src', this.NEED_TEST_ICON_URI);
          image.setAttribute('class', 'need');
          untested.appendChild(image);

          if (1 == needCount) {
            testLink.setAttribute('title', '1 test needs results from your client, please click here to run test');
          }
          else {
            testLink.setAttribute('title', needCount + ' tests need results from your client, please click here to run tests');
          }
          untested.appendChild(document.createTextNode(' ' + needCount + '\u00A0untested, please\u00A0test'));
          testLink.appendChild(untested);
        }
        heading.appendChild(testLink);
        annotation.appendChild(heading);

        // Engine result data
        if (! this.mClosed) {
          var majorEngines = document.createElement('div');
          var minorEngines = document.createElement('div');
          majorEngines.setAttribute('class', 'engines');
          minorEngines.setAttribute('class', 'engines');
          
          for (engineName in section.results) {
            if (section.results.hasOwnProperty(engineName)) {
              var engineResults = section.results[engineName];
              var resultCount = (engineResults.pass_count + engineResults.fail_count);
              
              var toolTip = '';
              var engineClass = '';
              if (0 < resultCount) {
                if (engineResults.pass_count == section.test_count) {
                  toolTip = ((1 < section.test_count)  ? 'All tests pass' : 'Test passed');
                  engineClass = 'pass';
                }
                else {
                  if (engineResults.fail_count == section.test_count) {
                    toolTip = ((1 < section.test_count)  ? 'All tests fail' : 'Test failed');
                    engineClass = 'epic-fail';
                  }
                  else {
                    if (0 < engineResults.pass_count) {
                      toolTip = engineResults.pass_count + ' pass';
                    }
                    if (0 < engineResults.fail_count) {
                      if (toolTip.length) {
                        toolTip += ', '
                      }
                      toolTip += engineResults.fail_count + ' fail';
                    }
                    if (resultCount < section.test_count) {
                      if (toolTip.length) {
                        toolTip += ', '
                      }
                      toolTip += (section.test_count - resultCount) + ' untested';
                    }
                    if ((resultCount / section.test_count) < 0.90) {
                      engineClass = 'untested';
                    }
                    else {
                      switch (Math.round((engineResults.pass_count / section.test_count) * 10.0)) {
                        case 10:
                        case 9: engineClass = 'almost-pass';  break;
                        case 8: engineClass = 'slightly-buggy'; break;
                        case 7: engineClass = 'buggy'; break;
                        case 6: engineClass = 'very-buggy'; break;
                        case 5: engineClass = 'fail'; break;
                        default: engineClass = 'epic-fail'; break;
                      }
                    }
                  }
                }
              }
              else {
                toolTip = 'No data';
              }
              
              if (0 < resultCount) {
                var engineNode = document.createElement('span');
                engineNode.setAttribute('title', toolTip);
                if (this.mResponse.engine_titles[engineName] == this.mResponse.client_engine) {
                  engineClass += ' active';
                }
                engineNode.setAttribute('tabindex', '0');
                engineNode.setAttribute('data-engineName', engineName);
                engineNode.setAttribute('data-passCount', engineResults.pass_count);
                engineNode.setAttribute('data-failCount', engineResults.fail_count);
                engineNode.setAttribute('data-needCount', section.test_count - resultCount);
                engineNode.onclick = function(domEvent) { annotator.toggleDetails(domEvent); };

                if (engineName in this.ENGINE_LOGOS) {
                  engineClass += ' major';
                  var logo = document.createElement('img');
                  logo.setAttribute('src', this.ENGINE_LOGOS[engineName]);
                  engineNode.appendChild(logo);
                  majorEngines.appendChild(engineNode);
                  majorEngines.appendChild(document.createTextNode(' '));
                }
                else {
                  engineNode.appendChild(document.createTextNode(this.mResponse.engine_titles[engineName]));
                  minorEngines.appendChild(engineNode);
                  minorEngines.appendChild(document.createTextNode(' '));
                }

                engineNode.setAttribute('class', engineName.substr(1) + ' ' + engineClass);
              }
            }
          }
          annotation.appendChild(majorEngines);
          annotation.appendChild(minorEngines);
          
          var details = document.createElement('div');
          details.setAttribute('class', 'details');
          
          var engineName = document.createElement('div');
          engineName.setAttribute('class', 'engine');
          engineName.appendChild(document.createTextNode('engine '));
          
          var detailsLink = document.createElement('a');
          detailsLink.appendChild(document.createTextNode('test details'));
          engineName.appendChild(detailsLink);
          details.appendChild(engineName);
          
          var meter = document.createElement('div');
          meter.setAttribute('class', 'meter');
          for (barClass in { 'numbers': '', 'pass': '', 'epic-fail': '', 'untested': '' }) {
            var bar = document.createElement('span');
            bar.setAttribute('class', barClass);
            meter.appendChild(bar);
          }
          details.appendChild(meter);
          
          annotation.appendChild(details);
        }
        
        targetElement.insertBefore(annotation, targetElement.firstChild);
        return true;
      }
    }
    catch (err) {
//      document.body.innerHTML = 'EXCEPTION: ' + err.toString() + err.lineNumber; // DEBUG
    }
    return false;
  },
  
  addAnnotation: function(section, first) {
    try {
      var anchorName = section.anchor_name;

      if (anchorName) { // find element that has anchor name or id
        var found = false;

        anchor = document.getElementById(anchorName);
        if (! (anchor && this.addAnnotationTo(anchor, section, first))) {
          var anchors = document.getElementsByName(anchorName);
          
          for (index in anchors) {
            var anchor = anchors[index];
            if (this.addAnnotationTo(anchor, section, first)) {
              break;
            }
          }
        }
      }
      else if (first) {  // find first h1
        var headings = document.getElementsByTagName('h1');
        
        if (headings && (0 < headings.length)) {
          this.addAnnotationTo(headings[0], section, first);
        }
      }
    }
    catch (err) {
    }
  },
  
  addAnnotations: function () {
    try {
      if (this.mResponse && this.mResponse.sections) {
        if (this.mClosed) {
          this.addAnnotation(this.mResponse.sections['_'], true);
        }
        else {
          var first = true;
          for (anchorName in this.mResponse.sections) {
            if (this.mResponse.sections.hasOwnProperty(anchorName)) {
              this.addAnnotation(this.mResponse.sections[anchorName], first);
              first = false;
            }
          }
        }
      }
    }
    catch (err) {
    }
  },
  
  processResponse: function(contentType, responseText) {
    try {
      if (-1 < contentType.indexOf('application/vnd.csswg.harness.v1+json')) {
        var response = JSON.parse(responseText);

        if (response) {
          this.mResponse = response;
          this.addAnnotations();
        }
      }
    }
    catch (err) {
    }
  },
  
  annotate: function() {
    try {
      var testSuiteName = '';
      
      var scripts = document.getElementsByTagName('script');
      for (index in scripts) {
        if (scripts[index].hasAttribute('src')) {
          var scriptSource = scripts[index].getAttribute('src');
          if (-1 < scriptSource.indexOf('/annotate.js#')) {
            testSuiteName = scriptSource.substr(scriptSource.indexOf('#') + 1);
            if ('!' == testSuiteName[0]) {
              testSuiteName = testSuiteName.substr(1);
              this.mClosed = true;
            }
            break;
          }
        }
      }
      
      if (0 < testSuiteName.length) {
        var styleSheet = document.createElement('link');
        styleSheet.setAttribute('rel', 'stylesheet');
        styleSheet.setAttribute('type', 'text/css');
        styleSheet.setAttribute('href', this.STYLESHEET_URI);
        document.getElementsByTagName('head')[0].appendChild(styleSheet)
        
        var specName = '';
        var slashIndex = testSuiteName.indexOf('/');
        if (-1 < slashIndex) {
          specName = testSuiteName.substr(slashIndex + 1);
          testSuiteName = testSuiteName.substr(0, slashIndex);
        }

        var statusURI = this.QUERY_URI +
                            '?suite=' + encodeURIComponent(testSuiteName) +
                            '&spec=' + encodeURIComponent(specName) +
                            '&uri=' + encodeURIComponent(document.URL);
        if (window.XDomainRequest) {  // The IE way...
          var xdr = new XDomainRequest();
          if (xdr) {
            xdr.onload = function () {
              annotator.processResponse(xdr.contentType, xdr.responseText);
            }
            xdr.open('GET', statusURI);
            xdr.send();
          }
        }
        else {  // The standard way
          var xhr = new XMLHttpRequest();
          
          xhr.onreadystatechange = function() {
            if (4 == xhr.readyState) {
              if (200 == xhr.status) {
                annotator.processResponse(xhr.getResponseHeader('Content-Type'), xhr.responseText);
              }
              else if (500 == xhr.status) {
//                document.documentElement.innerHTML = xhr.responseText;  // DEBUG
              }
              else {
//                document.body.innerHTML = 'error: ' + xhr.status + xhr.responseText; // DEBUG
              }
            }
          };
          
          xhr.open('GET', statusURI, true);
          xhr.setRequestHeader('Accept', 'application/vnd.csswg.harness.v1+json');
          xhr.send();
        }
      }
    }
    catch (err) {
//      document.body.innerHTML = 'EXCEPTION: ' + err.toString(); // DEBUG
    }
  },
  
  addLoadEvent: function() {
    try {
      var oldOnLoad = window.onload;
      if (typeof window.onload != 'function') {
        window.onload = this.annotate();
      }
      else {
        window.onload = function () {
          if (oldOnLoad) {
            oldOnLoad();
          }
          annotator.annotate();
        }
      }
    }
    catch (err) {
    }
  }
}


annotator.addLoadEvent();

