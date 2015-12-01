exports.hide = function () {
  var webEditRibbonEl = document.getElementById('scWebEditRibbon');
  if (typeof (webEditRibbonEl) != 'undefined' && webEditRibbonEl != null) {
    webEditRibbonEl.parentNode.removeChild(webEditRibbonEl);
  }

  var crossPiece = document.getElementById('scCrossPiece');
  if (typeof (crossPiece) != 'undefined' && crossPiece != null) {
    crossPiece.parentNode.removeChild(crossPiece);
  }

  var preNav = document.getElementById('ribbonPreLoadingIndicator');
  if (typeof (preNav) != 'undefined' && preNav != null) {
    preNav.parentNode.removeChild(preNav);
  }
};

exports.initHighlight = function () {
  if (typeof (Sitecore) != 'undefined') {
    Sitecore.PageModes.Utility.addStyleSheet('.scHighlightTestingFrameHorizontal { background: red; height: 4px; position: absolute; z-index: 9000;  }');
    Sitecore.PageModes.Utility.addStyleSheet('.scHighlightTestingFrameVertical { background: red; width: 4px; position: absolute; z-index: 9000;  }');

    Sitecore.PageModes.HighlightTestingFrame = Sitecore.PageModes.ChromeFrame.extend({
      constructor: function () {
        this.base();
        this.sideWidth = 4;
        this.createSides();
      },

      createSides: function () {
        this.top = $sc('<div></div>').addClass(this.horizontalSideClassName());
        this.right = $sc('<div></div>').addClass(this.verticalSideClassName());
        this.bottom = $sc('<div></div>').addClass(this.horizontalSideClassName());
        this.left = $sc('<div></div>').addClass(this.verticalSideClassName());

        var sides = new Array();
        this.sides = sides;

        sides.push(this.top);
        sides.push(this.right);
        sides.push(this.bottom);
        sides.push(this.left);

        this.base();
      },

      horizontalSideClassName: function () {
        return 'scHighlightTestingFrameHorizontal';
      },

      verticalSideClassName: function () {
        return 'scHighlightTestingFrameVertical';
      },

      showSides: function (chrome) {
        var offset = chrome.position.offset();
        var dimensions = chrome.position.dimensions();

        var horizontalX = offset.left - this.sideWidth;
        var verticalLeftX = offset.left - this.sideWidth;
        var verticalRightX = offset.left + dimensions.width;

        var topY = offset.top - this.sideWidth;
        var bottomY = offset.top + dimensions.height;
        var verticalY = offset.top;

        this.setSideStyle(this.top, topY, horizontalX, dimensions.width + (this.sideWidth * 2));
        this.setSideStyle(this.right, verticalY, verticalRightX, dimensions.height);
        this.setSideStyle(this.bottom, bottomY, horizontalX, dimensions.width + (this.sideWidth * 2));
        this.setSideStyle(this.left, verticalY, verticalLeftX, dimensions.height);

        this.base(chrome);
      },

      dispose: function () {
        if (this.sides) {
          $sc.each(this.sides, function () {
            this.remove();
          });
        }

        this.sides = null;
      }
    });
  }
};

exports.highlight = function (mvtids, condids, varids) {
  if(typeof(Sitecore) != 'undefined') {
        
    for(i in Sitecore.PageModes.ChromeManager.chromes()){
      var c = Sitecore.PageModes.ChromeManager.chromes()[i];
      if(c && c.data && c.data.custom && c.data.custom.testVariations) {
        for(ii in c.data.custom.testVariations){
          var v = c.data.custom.testVariations[ii];
          if (mvtids.indexOf(v.id) >= 0 || varids.indexOf(v.id) >= 0) {
            c.showHighlight(new Sitecore.PageModes.HighlightTestingFrame());
          }
        }
      }
		
      if(c && c.type && c.type instanceof Sitecore.PageModes.ChromeTypes.Rendering && c.type.getConditions){
        for(ii in c.type.getConditions()){
          var cond = c.type.getConditions()[ii];
          if (condids.indexOf(cond.id) >= 0 || varids.indexOf(cond.id) >= 0) {
            c.showHighlight(new Sitecore.PageModes.HighlightTestingFrame());
          }
        }
      }

      if(c && c.type && c.type instanceof Sitecore.PageModes.ChromeTypes.Field && c.type.parameters && c.type.parameters["sc-highlight-contentchange"] == "yes") {
        c.showHighlight(new Sitecore.PageModes.HighlightTestingFrame());
      }
    }
  }
};