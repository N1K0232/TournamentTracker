function tournaments(language) {
    Alpine.data("tournaments", () => ({

        name: '',
        entryFee: 0.0,
        startDate: '',
        pageIndex: 1,
        itemsPerPage: 10,
        tournaments: [],
        totalCount: 0,
        hasNextPage: false,
        isBusy: false,

        reset: function () {
            this.name = '';
            this.entryFee = 0.0;
            this.startDate = '';
            this.pageIndex = 0;
            this.itemsPerPage = 50;
            this.tournaments = [];
            this.totalCount = 0;
            this.hasNextPage = false;
            this.isBusy = false;
        },

        create: async function () {

            this.isBusy = true;

            try {

                const request = { name: this.name, entryFee: this.entryFee, startDate: this.startDate }
                const response = await fetch("/api/tournaments", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                const content = await response.json();
                const errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {

                    document.cookie = content.id;
                    window.location.href = '/';
                }
                else {
                    alert(errorMessage);
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        get: async function (id) {

            this.isBusy = true;

            try {

                const response = await fetch(`/api/tournaments/${id}`, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                });

                const content = await response.json();
                const errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    this.name = content.name;
                    this.entryFee = content.entryFee;
                    this.startDate = content.startDate;
                }
                else {
                    alert(errorMessage);
                }
            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        getList: async function () {

            this.isBusy = true;

            try {

                const response = await fetch(`/api/tournaments?name=${this.name}&pageIndex=${this.pageIndex - 1}&itemsPerPage=${this.itemsPerPage}`, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    }
                });

                const content = await response.json();
                const errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    this.tournaments = content.content;
                    this.totalCount = content.totalCount;
                    this.hasNextPage = content.hasNextPage;
                }
                else {
                    alert(errorMessage);
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        delete: async function (id) {

            this.isBusy = true;

            try {
                const response = await fetch(`/api/tournaments/${id}`, {
                    method: "DELETE",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                });

                const content = await response.json();
                const errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    window.location.href = '/';
                }
                else {
                    alert(errorMessage);
                }
            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        update: async function (id) {

            this.isBusy = true;

            try {

                const request = { name: this.name, entryFee: this.entryFee, startDate: this.startDate };
                const response = await fetch(`/api/tournaments/${id}`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                const content = await response.json();
                const errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    window.location.href = '/';
                }
                else {
                    alert(errorMessage);
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        invalidForm: function () {
            return this.name.trim().length == 0 && entryFee <= 0 && startDate.trim().length == 0;
        }
    }));
}