var app = angular.module('app');
app.controller('dashboardCtrl', ['$scope', '$http', 'chartService', '$routeParams', function ($scope, $http, chartService, $routeParams) {

    $scope._chartService = chartService;
    $scope.subscriptions = [];

    //gives nearset hundred e.g.if amount is 789 then 800, if amount is 902 then 1000.
    var nearestHundred = function (amount) {
        var nextHundred = 0;
        nextHundred = (amount / 100);
        nextHundred += 1;
        return nextHundred * 100;
    }

    $scope.initGraph = function () {

        $scope.monthId = $routeParams.monthId !== undefined ? $routeParams.monthId : ''

        $http({
            method: 'GET',
            url: '/data/SpendingBySubscriptionDaily?monthId=' + $scope.monthId
        }).then(function successCallback(response) {
            var data = {};
            data.title = 'Daily Usage',
            data.categories = response.data.date;
            data.series = response.data.series;
            $scope._chartService.drawLineChart('container2', data);
        });

        $http({
            method: 'GET',
            url: '/data/SpendingBySubscription?monthId=' + $scope.monthId
        }).then(function successCallback(response) {

            // load subscription master list
            $scope.subscriptions = [];
            var itemCount = 0;
            var amount = 0.0;
            for (index in response.data) {
                itemCount++;
                $scope.subscriptions[$scope.subscriptions.length] = { id: itemCount, text: response.data[index].name };
                amount += response.data[index].y;
            }

            // half PieChart Data
            var halfPieChartData = {};
            halfPieChartData.title = 'Total consumption MTD';
            halfPieChartData.totalCostSummary = '$' + Math.round(amount * 100) / 100;
            halfPieChartData.data = [amount, nearestHundred(amount) - amount];
            $scope._chartService.drawHalfPieChart('container1', halfPieChartData);

            // draw bar chart
            var barChartData = {};
            barChartData.title = 'Consumption by Subscription',
            barChartData.data = response.data
            barChartData.series = response.data.data;
            $scope._chartService.drawBarChart('container4', barChartData);
        });
    };

    $scope.export = function (name) {
        var chart = $("#" + name).highcharts();
        chart.exportChart({
            type: 'application/pdf',
            filename: 'my-pdf'
        });
    };

    $scope.initGraph();
}]);