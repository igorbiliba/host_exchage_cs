<?php

$clients = [
	[
	 	'./host_exchage_cs/bin/Release/ticket_clients/Bart',
	 	'./../netex_client_cs/netex_client_cs/bin/Release'
	],
	[
		'./host_exchage_cs/bin/Release/ticket_clients/Lisa',
		'./../365cash_client_cs/365cash_client_cs/bin/Release'
	],
	[
		'./host_exchage_cs/bin/Release/ticket_clients/Maggie',
		'./../mine_exchange_cs/mine_exchange_cs/bin/Release'
	],
	[
		'./host_exchage_cs/bin/Release/ticket_clients/KonvertIm',
		'./../konvert_im_cs/KonvertIm/KonvertIm/bin/Release'
	],
];

$remove = [
	'./host_exchage_cs/bin/Release/db.db3',
	'./host_exchage_cs/bin/Release/ssl.cert',
	'./host_exchage_cs/bin/Release/Test.json',
	
	'./host_exchage_cs/bin/Release/ticket_clients/Maggie/db.db3',
	'./host_exchage_cs/bin/Release/ticket_clients/Lisa/db.db3',
	'./host_exchage_cs/bin/Release/ticket_clients/Bart/db.db3',
	'./host_exchage_cs/bin/Release/ticket_clients/KonvertIm/db.db3',
	
	'./host_exchage_cs/bin/Release/ticket_clients/Maggie/ProxyLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/Lisa/ProxyLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/Bart/ProxyLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/KonvertIm/ProxyLog.json',
	
	'./host_exchage_cs/bin/Release/ticket_clients/Maggie/ActionsLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/Lisa/ActionsLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/Bart/ActionsLog.json',
	'./host_exchage_cs/bin/Release/ticket_clients/KonvertIm/ActionsLog.json',
	
	'./host_exchage_cs/bin/Release/ticket_clients/Lisa/istest',
	'./host_exchage_cs/bin/Release/ticket_clients/Bart/istest',	
	'./host_exchage_cs/bin/Release/ticket_clients/Maggie/istest',
	'./host_exchage_cs/bin/Release/ticket_clients/KonvertIm/istest',
	
	'./host_exchage_cs/bin/Release/tmp/Bestchange.zip_extracted',
	'./host_exchage_cs/bin/Release/tmp/Bestchange.zip'
];

$push = implode(' && ', [
	'git pull origin master',
	'git add .',
	'git commit -am "RELEASE: pre assembly"',
	'git push origin master'
]);

$zip = '"C:/Program Files/7-Zip/7z.exe" a -tzip -mx5 -r0 ./host_exchage_cs/bin/build.zip ./host_exchage_cs/bin/Release/';

///////////////////////////////////////////////////////////////////////////////////////////

//1.
foreach($clients as $client)
{
	if(is_dir($client[0]))
		delete_directory($client[0]);
	
	recurse_copy($client[1], $client[0]);
}

//2.
foreach($remove as $path)
{
	if(is_dir($path)) {
		delete_directory($path);
	} else if(file_exists($path)) {
		unlink($path);
	}
}

//3.
exec($push);

//4.
if(file_exists('./host_exchage_cs/bin/build.zip'))
{
	unlink('./host_exchage_cs/bin/build.zip');
}
exec($zip);

//5.
exec("start chrome https://github.com/igorbiliba/host_exchage_cs/releases/new");
exec("start explorer .\\host_exchage_cs\\bin");




























///////////////////////////////////////////////////////////////////////////////////////////
function delete_directory($dir) {
    if (!file_exists($dir)) {
        return true;
    }

    if (!is_dir($dir)) {
        return unlink($dir);
    }

    foreach (scandir($dir) as $item) {
        if ($item == '.' || $item == '..') {
            continue;
        }

        if (!delete_directory($dir . '/' . $item)) {
            return false;
        }

    }

    return rmdir($dir);
}
function recurse_copy($src,$dst) { 
    $dir = opendir($src); 
    @mkdir($dst); 
    while(false !== ( $file = readdir($dir)) ) { 
        if (( $file != '.' ) && ( $file != '..' )) { 
            if ( is_dir($src . '/' . $file) ) { 
                recurse_copy($src . '/' . $file,$dst . '/' . $file); 
            } 
            else { 
                copy($src . '/' . $file,$dst . '/' . $file); 
            } 
        } 
    } 
    closedir($dir); 
} 
