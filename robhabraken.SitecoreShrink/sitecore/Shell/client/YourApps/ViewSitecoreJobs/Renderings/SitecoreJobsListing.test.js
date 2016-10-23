require(["jasmineEnv"], function (jasmineEnv) {
  var setupTests = function () {
    "use strict";

    describe("Given a SitecoreJobsListing model", function () {
      var component = new Sitecore.Definitions.Models.SitecoreJobsListing();

      describe("when I create a SitecoreJobsListing model", function () {
        it("it should have a 'isVisible' property that determines if the SitecoreJobsListing component is visible or not", function () {
          expect(component.get("isVisible")).toBeDefined();
        });

        it("it should set 'isVisible' to true by default", function () {
          expect(SitecoreJobsListing.get("isVisible")).toBe(true);
        });

        it("it should have a 'toggle' function that either shows or hides the SitecoreJobsListing component depending on the 'isVisible' property", function () {
          expect(component.toggle).toBeDefined();
        });
      });
    });
  };

  runTests(jasmineEnv, setupTests);
});