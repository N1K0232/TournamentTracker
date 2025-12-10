function createTournament(language)
{
    Alpine.data("tournaments", () => ({
        name: '',
        entryFee: 0,
        startsAt: '',
        endsAt: '',
        isBusy: false,
        errorMessage: '',

        create: async function ()
        {
            this.isBusy = true;

            try
            {
                const response = await createTournamentAsync(this.name, parseFloat(this.entryFee), this.startsAt, this.endsAt, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {
                    window.location.href = `/Tournaments/Created/${content.id}`;
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

async function createTournamentAsync(name, entryFee, startsAt, endsAt, language)
{
    const request = {
        name: name,
        entryFee: entryFee,
        startsAt: startsAt ? new Date(startsAt).toISOString() : null,
        endsAt: endsAt ? new Date(endsAt).toISOString() : null
    };

    const response = await fetch('/api/tournaments', {
        method: "POST",
        headers: {
            "Accept-Language": language,
            "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
    });

    return response;
}