angular.module('virtoCommerce.licensingModule')
.controller('virtoCommerce.licensingModule.licenseDetailController', ['$scope', 'virtoCommerce.licensingModule.licenseApi', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService',
function ($scope, licenseApi, dialogService, bladeNavigationService) {
    var blade = $scope.blade;
    blade.updatePermission = 'licensing:update';
    blade.isNew = !blade.data.id;

    blade.initialize = function (data) {
        blade.origEntity = data;
        blade.currentEntity = angular.copy(data);
        blade.isLoading = false;
    };

    $scope.cancelChanges = function () {
        blade.currentEntity = blade.origEntity;
        $scope.bladeClose();
    };
    $scope.saveChanges = function () {
        blade.isLoading = true;

        licenseApi.save(blade.currentEntity, function () {
            angular.copy(blade.currentEntity, blade.origEntity);
            $scope.bladeClose();
        });
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope.$valid;
    }

    $scope.setForm = function (form) { $scope.formScope = form; };

    if (blade.isNew) {
        angular.extend(blade, {
            title: 'licensing.blades.license-detail.title-new'
        });
    } else {
        angular.extend(blade, {
            title: 'licensing.blades.license-detail.title'
        });
    }

    blade.initialize(blade.data);
}]);