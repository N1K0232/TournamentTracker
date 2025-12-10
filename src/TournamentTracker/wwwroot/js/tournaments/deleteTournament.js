function deleteTournament(language)
{
    Alpine.data("tournaments", () => ({
        tournament: {
            id: '',
            name: '',
            entryFee: 0.0,
            startsAt: '',
            endsAt: ''
        },
        isBusy: false,
        errorMessage: '',

        get: async function (id)
        {
            this.isBusy = true;

            try
            {
                const response = await getTournamentAsync(id, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {
                    this.tournament = content;
                }
            }
            catch (error)
            {
                this.errorMessage = error.message;
            }
            finally
            {
                this.isBusy = false;
            }
        },

        delete: async function (id)
        {
            this.isBusy = true;

            try
            {
                const response = await deleteTournamentAsync(id, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {
                    window.location.href = '/';
                }
            }
            catch(error)
            {
                this.errorMessage = error.message;
            }
            finally
            {
                this.isBusy = false;
            }
        }
    }));
}

async function getTournamentAsync(id, language)
{
    const response = await fetch(`/api/tournaments/${id}`, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}

async function deleteTournamentAsync(id, language)
{
    const response = await fetch(`/api/tournaments/${id}`, {
        method: "DELETE",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}