angular.module('virtoCommerce.licensingModule')
    .factory('virtoCommerce.licensingModule.licenseApi', ['$resource', function ($resource) {
        return $resource('api/licenses', null, {
            search: { method: 'POST', url: 'api/licenses/search' }
        });
    }]);