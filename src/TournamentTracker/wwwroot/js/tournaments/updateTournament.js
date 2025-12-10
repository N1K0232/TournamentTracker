function updateTournament(language)
{
    Alpine.data("tournaments", () => ({
        tournament: {
            id: '',
            name: '',
            entryFee: 0.0,
            startsAt: '',
            endsAt: ''
        },
        name: '',
        entryFee: 0.0,
        startsAt: '',
        endsAt: '',
        isBusy: false,
        errorMessage: '',

        get: async function (id)
        {
            this.isBusy = true;

            try
            {
                const response = await getTournamentAsync(id, language);
                this.tournament = await response.json();
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

        update: async function (id)
        {
            this.isBusy = true;

            try
            {
                const response = await createTournamentAsync(id, this.name, this.entryFee, this.startsAt, this.endsAt, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {

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
        }
    }));
}

async function getTournamentAsync(id, language) {
    const response = await fetch(`/api/tournaments/${id}`, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}

async function updateTournamentAsync(id, name, entryFee, startsAt, endsAt, language)
{
    const request = {
        name: name,
        entryFee: entryFee,
        startsAt: startsAt ? new Date(startsAt).toISOString() : null,
        endsAt: endsAt ? new Date(endsAt).toISOString() : null
    };

    const response = await fetch(`/api/tournaments/${id}`, {
        method: "PUT",
        headers: {
            "Accept-Language": language,
            "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
    });

    return response;
}