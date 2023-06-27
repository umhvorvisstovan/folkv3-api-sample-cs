using Us.FolkV3.Api.Cert;
using Us.FolkV3.Api.Client;
using Us.FolkV3.Api.Model;
using Us.FolkV3.Api.Model.Param;

namespace Us.FolkV3.ApiSample;

class Sample
{
    private readonly HeldinConfig _heldinConfig;
    private readonly CertificateConfig _certificateConfig;
    private PersonSmallClient _smallClient;
    private PersonMediumClient _mediumClient;
    private PrivateCommunityClient _privateCommunityClient;
    private PublicCommunityClient _publicCommunityClient;

    public Sample(HeldinConfig heldinConfig, CertificateConfig certificateConfig = null)
    {
        _heldinConfig = heldinConfig;
        _certificateConfig = certificateConfig;
    }

    static void Main(string[] args)
    {
        var heldinConfig = HeldinConfig.ForSecureHost("10.20.30.40")
            .Fo().Test() // Or Prod()
            .Com() // Or Gov()
            .MemberCode("123456")
            .SubSystemCode("my-system")
            .WithUserId("my-system-id"); // Optional userId
        // Or like this
		// var heldinConfig = HeldinConfig.Create("10.20.30.40", true, "FO-TST/COM/123456/my-system");

        // Load certificate configuration from AppSettings or ENV properties
        //   AppSettings name               ENV name
        //   FolkV3.TlsProtocol             FOLKV3_TLSPROTOCOL             optional
        //   FolkV3.ClientKeyStore.Path     FOLKV3_CLIENTKEYSTORE_PATH
        //   FolkV3.ClientKeyStore.Password FOLKV3_CLIENTKEYSTORE_PASSWORD
        //   FolkV3.serverCertificate.Path  FOLKV3_SERVERCERTIFICATE_PATH  optional, default trust all
        //
        //   ENV properties
        //   FOLKV3_TLSPROTOCOL=TLSv13; FOLKV3_CLIENTKEYSTORE_PATH=/path/to/client-cert.pfx; FOLKV3_CLIENTKEYSTORE_PASSWORD=???; FOLKV3_SERVERCERTIFICATE_PATH=/path/to/server-cert.cer;
        //
		// var certificateConfig = CertificateConfig.LoadClientCertificate();
		// var certificateConfig = CertificateConfig.LoadServerCertificate();
		// var certificateConfig = CertificateConfig.LoadClientAndServerCertificate();

        var certificateConfig = CertificateConfig.Builder()
            .ClientKeyStorePath(@"/path/to/client-cert.pfx") // Required if the X-Road client (SUBSYSTEM:FO-TST/COM/123456/my-system) specifies HTTPS
            .ClientKeyStorePassword("???")
            .ServerCertificatePath(@"/path/to/server-cert.cer") // Optional, default trust all
            .TlsProtocol(TlsProtocolVersion.TLSv13) // Optional, default value TLSv13
            .Build();

        var sample = new Sample(heldinConfig, certificateConfig);
        // Use this if there is no client certificate and/or you trust server certificate
		// var sample = new Sample(heldinConfig);

        sample.TestGetPersonMediumByPtal();
    }

    private void Call(Action method)
    {
        try
        {
            method.Invoke();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine();
        }
    }

    private void TestSmallMethods()
    {
        Call(TestGetPersonSmallByPrivateId);
        Call(TestGetPersonSmallByPtal);
        Call(TestGetPersonSmallByNameAndAddress);
        Call(TestGetPersonSmallByNameAndDateOfBirth);
    }

    private void TestMediumMethods()
    {
        Call(TestGetPersonMediumByPrivateId);
        Call(TestGetPersonMediumByPublicId);
        Call(TestGetPersonMediumByPtal);
        Call(TestGetPersonMediumByNameAndAddress);
        Call(TestGetPersonMediumByNameAndDateOfBirth);
    }

    private void TestPrivateCommunityMethods()
    {
        Call(TestGetPrivateChanges);
        Call(TestAddPersonToCommunityByNameAndAddress);
        Call(TestAddPersonToCommunityByNameAndDateOfBirth);
        Call(TestRemovePersonFromCommunity);
        Call(TestRemovePersonsFromCommunity);
    }

    private void TestPublicCommunityMethods()
    {
        Call(TestGetPublicChanges);
    }

    private PersonSmallClient SmallClient()
    {
        if (_smallClient == null)
        {
            _smallClient = FolkClient.PersonSmall(_heldinConfig, _certificateConfig);
        }
        return _smallClient;
    }

    private PersonMediumClient MediumClient()
    {
        if (_mediumClient == null)
        {
            _mediumClient = FolkClient.PersonMedium(_heldinConfig, _certificateConfig);
        }
        return _mediumClient;
    }

    private PrivateCommunityClient PrivateCommunityClient()
    {
        if (_privateCommunityClient == null)
        {
            _privateCommunityClient = FolkClient.PrivateCommunity(_heldinConfig, _certificateConfig);
        }
        return _privateCommunityClient;
    }

    private PublicCommunityClient PublicCommunityClient()
    {
        if (_publicCommunityClient == null)
        {
            _publicCommunityClient = FolkClient.PublicCommunity(_heldinConfig, _certificateConfig);
        }
        return _publicCommunityClient;
    }


    // Test private methods

    private void TestSmallGetMyPrivileges()
    {
        Console.WriteLine("# TestSmallGetMyPrivileges");
        SmallClient().GetMyPrivileges().ToList().ForEach(Console.WriteLine);
    }

    private void TestGetPersonSmallByPrivateId()
    {
        Console.WriteLine("# TestGetPersonSmallByPrivateId");
        var person = SmallClient().GetPerson(
                PrivateId.Create(1)
                );
        PrintPerson(person);
    }

    private void TestGetPersonSmallByPtal()
    {
        Console.WriteLine("# TestGetPersonSmallByPtal");
        var person = SmallClient().GetPerson(
                Ptal.Create("300408559")
                );
        PrintPerson(person);
    }

    private void TestGetPersonSmallByNameAndAddress()
    {
        Console.WriteLine("# TestGetPersonSmallByNameAndAddress");
        var person = SmallClient().GetPerson(
                NameParam.Create("Karius", "Davidsen"),
                AddressParam.Create("Úti í Bø",
                        HouseNumber.Create(16),
                        "Syðrugøta")
                );
        PrintPerson(person);
    }

    private void TestGetPersonSmallByNameAndDateOfBirth()
    {
        Console.WriteLine("# TestGetPersonSmallByNameAndDateOfBirth");
        var person = SmallClient().GetPerson(
                NameParam.Create("Karius", "Davidsen"),
                new DateTime(2008, 4, 30)
                );
        PrintPerson(person);
    }


    // Test public methods

    private void TestMediumGetMyPrivileges()
    {
        Console.WriteLine("# TestMediumGetMyPrivileges");
        MediumClient().GetMyPrivileges().ToList().ForEach(Console.WriteLine);
    }

    private void TestGetPersonMediumByPrivateId()
    {
        Console.WriteLine("# TestGetPersonMediumByPrivateId");
        var person = MediumClient().GetPerson(
                PrivateId.Create(1)
                );
        PrintPerson(person);
    }

    private void TestGetPersonMediumByPublicId()
    {
        Console.WriteLine("# TestGetPersonMediumByPublicId");
        var person = MediumClient().GetPerson(
                PublicId.Create(1157442)
                );
        PrintPerson(person);
    }

    private void TestGetPersonMediumByPtal()
    {
        Console.WriteLine("# TestGetPersonMediumByPtal");
        var person = MediumClient().GetPerson(
                Ptal.Create("300408559")
                );
        PrintPerson(person);
    }

    private void TestGetPersonMediumByNameAndAddress()
    {
        Console.WriteLine("# TestGetPersonMediumByNameAndAddress");
        var person = MediumClient().GetPerson(
                NameParam.Create("Karius", "Davidsen"),
                AddressParam.Create("Úti í Bø",
                        HouseNumber.Create(16),
                        "Syðrugøta")
                );
        PrintPerson(person);
    }

    private void TestGetPersonMediumByNameAndDateOfBirth()
    {
        Console.WriteLine("# TestGetPersonMediumByNameAndDateOfBirth");
        var person = MediumClient().GetPerson(
                NameParam.Create("Karius", "Davidsen"),
                new DateTime(2008, 4, 30)
                );
        PrintPerson(person);
    }


    // Test community methods

    private void TestGetPrivateChanges()
    {
        Console.WriteLine("# TestGetPrivateChanges");
        Changes<PrivateId> changes = PrivateCommunityClient().GetChanges(DateTime.Now.AddDays(-7));
        Console.WriteLine("Changes - from: {0}; to: {1}; ids: [{2}]\n", changes.From, changes.To, string.Join(", ", changes.Ids));
    }

    private void TestGetPublicChanges()
    {
        Console.WriteLine("# TestGetPublicChanges");
        Changes<PublicId> changes = PublicCommunityClient().GetChanges(DateTime.Now.AddDays(-7));
        Console.WriteLine("Changes - from: {0}; to: {1}; ids: [{2}]\n", changes.From, changes.To, string.Join(", ", changes.Ids));
    }

    private void TestAddPersonToCommunityByNameAndAddress()
    {
        Console.WriteLine("# TestAddPersonToCommunityByNameAndAddress");
        var communityPerson = PrivateCommunityClient().AddPersonToCommunity(
                NameParam.Create("Karius", "Davidsen"),
                AddressParam.Create("Úti í Bø",
                        HouseNumber.Create(16),
                        "Syðrugøta")
                );
        PrintCommunityPerson(communityPerson);
    }

    private void TestAddPersonToCommunityByNameAndDateOfBirth()
    {
        Console.WriteLine("# TestAddPersonToCommunityByNameAndDateOfBirth");
        var communityPerson = PrivateCommunityClient().AddPersonToCommunity(
                NameParam.Create("Karius", "Davidsen"),
                new DateTime(2008, 4, 30)
                );
        PrintCommunityPerson(communityPerson);
    }

    private void TestRemovePersonFromCommunity()
    {
        Console.WriteLine("# TestRemovePersonFromCommunity");
        var removedId = PrivateCommunityClient().RemovePersonFromCommunity(PrivateId.Create(1));
        Console.WriteLine("Removed id: {0}\n", removedId);
    }

    private void TestRemovePersonsFromCommunity()
    {
        Console.WriteLine("# TestRemovePersonsFromCommunity");
        var removedIds = PrivateCommunityClient().RemovePersonsFromCommunity(PrivateId.Create(1, 2, 3));
        Console.WriteLine("Removed ids: [{0}]\n", string.Join(", ", removedIds));
    }


    // Print methods

    private static void PrintPerson(PersonSmall person)
    {
        if (person == null)
        {
            Console.WriteLine("Person was not found!");
        }
        else
        {
            Console.WriteLine(PersonToString(person));
        }
        Console.WriteLine();
    }

    private static void PrintCommunityPerson(CommunityPerson person)
    {
        if (person == null)
        {
            Console.WriteLine("Oops!");
        }
        else
        {
            Console.WriteLine(CommunityPersonToString(person));
        }
        Console.WriteLine();
    }

    private static string PersonToString(PersonSmall person)
    {
        if (person.GetType() == typeof(PersonMedium)) {
            var personPublic = (PersonMedium)person;
            return Format(person.PrivateId, personPublic.PublicId, person.Name, AddressToString(person),
                    personPublic.DateOfBirth,
                    CivilStatusToString(personPublic), SpecialMarksToString(personPublic),
                    IncapacityToString(personPublic));
        }
        var deadOrAlive = person.IsAlive ? "ALIVE" : ("DEAD " + person.DateOfDeath);
        return Format(person.PrivateId, person.Name, AddressToString(person), deadOrAlive);
    }

    private static string CommunityPersonToString(CommunityPerson communityPerson)
    {
        string personString = null;
        if (communityPerson.IsAdded)
        {
            personString = PersonToString(communityPerson.Person);
        }
        return Format(communityPerson.Status, communityPerson.ExistingId, personString);
    }

    private static string AddressToString(PersonSmall person)
    {
        return AddressToString(person.Address);
    }

    private static string AddressToString(Address address)
    {
        return address.HasStreetAndNumbers
                        ? address.StreetAndNumbers
                                + "; " + address.Country.Code + address.PostalCode
                                + " " + address.City
                                + "; " + address.Country.NameFo
                                + " (From: " + address.From + ')'
                        : null;
    }

    private static string CivilStatusToString(PersonMedium person)
    {
        if (person.CivilStatus == null)
        {
            return null;
        }
        return person.CivilStatus.Type + ", " + person.CivilStatus.From;
    }

    private static string SpecialMarksToString(PersonMedium person)
    {
        return person.SpecialMarks.IsEmpty
                ? null : ('[' + string.Join(", ", person.SpecialMarks.Select(s => s.ToString())) + ']');
    }

    private static string IncapacityToString(PersonMedium person)
    {
        if (person.Incapacity == null)
        {
            return null;
        }
        var guardian1 = GuardianToString(person.Incapacity.Guardian1);
        var guardian2 = GuardianToString(person.Incapacity.Guardian2);
        return guardian2 == null ? guardian1 : guardian1 + " / " + guardian2;
    }

    private static string GuardianToString(Guardian guardian)
    {
        if (guardian == null)
        {
            return null;
        }
        return guardian.Name + " - " + AddressToString(guardian.Address);
    }

    private static string Format(params Object[] values)
    {
        return string.Join(
            " | ",
            values.ToList().Select(v => v == null ? "-" : v.ToString())
            );
    }

}
