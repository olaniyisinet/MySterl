// Code goes here
var app = angular.module('dcalc', []);
app.controller('doubble', function($scope) {

  $scope.calctitle = "Doubble Calculator";

  //Constants
  $scope.m3 = 36;
  $scope.m5 = 60;
  $scope.m10= 60;
  $scope.mls= 60;

  $scope.d3 = 36;
  $scope.d5 = 60;
  $scope.d10= 120;

  $scope.pd3 = 125;
  $scope.pd5 = 150;
  $scope.pd10= 200;
  $scope.pls = 200;

  $scope.saveamt = 1000;
  $scope.savels = 3000000;

  $scope.calDoubble = function(){

    //Total Savings
    $scope.tsd3 = $scope.saveamt * $scope.d3;
    $scope.tsd5 = $scope.saveamt * $scope.d5;
    $scope.tsd10= $scope.saveamt * ($scope.d10/2);

    //Monthly Payout
    $scope.mpd3 = $scope.saveamt * ($scope.pd3/100);
    $scope.mpd5 = $scope.saveamt * ($scope.pd5/100);
    $scope.mpd10= $scope.saveamt * (($scope.pd10/2)/100);

    //Total Earnings
    $scope.ted3 = $scope.mpd3 * $scope.d3;
    $scope.ted5 = $scope.mpd5 * $scope.d5;
    $scope.ted10= $scope.mpd10 * ($scope.d10);

    //Lump Sum Calculation
    // if($scope.savels < 1000000){$scope.savels = 1000000;}

    $scope.tmls = ($scope.savels*(200/100))/$scope.mls;
    $scope.tls = $scope.tmls*$scope.mls;

  };

  $scope.calDoubble();
});
