package no.miles.atmiles;

import android.app.Activity;
import android.app.ProgressDialog;
import android.app.SearchManager;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.SearchView;
import android.widget.Toast;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;

import no.miles.atmiles.employee.SearchResultModel;

public class EmployeeListActivity extends Activity
        implements EmployeeListFragment.Callbacks, SearchView.OnQueryTextListener {

    private ProgressDialog progress;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_employee_list);

        if (savedInstanceState != null) {
            restoreState(savedInstanceState);
        }

        progress = new ProgressDialog(this);
        progress.setTitle("Loading");
        progress.setMessage("Wait while loading...");
    }

    @Override
    protected void onResume() {
        super.onResume();
        //Clean to debug
        //new AuthenticationHelper().cleanLoginData(this);
        new AuthenticationHelper().checkLogin(this);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.home, menu);

        SearchManager searchManager = (SearchManager) getSystemService(Context.SEARCH_SERVICE);
        SearchView searchView = (SearchView) menu.findItem(R.id.action_search).getActionView();
        // Assumes current activity is the searchable activity
        searchView.setSearchableInfo(searchManager.getSearchableInfo(getComponentName()));
        searchView.setIconifiedByDefault(true);
        searchView.setOnQueryTextListener(this);



        return true;
    }

    /**
     * Callback method from {@link EmployeeListFragment.Callbacks}
     * indicating that the item with the given ID was selected.
     */
    @Override
    public void onItemSelected(String id) {
        Intent detailIntent = new Intent(this, EmployeeDetailActivity.class);
        detailIntent.putExtra(EmployeeDetailFragment.ARG_ITEM_ID, id);
        startActivity(detailIntent);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent authActivityResult) {
        super.onActivityResult(requestCode, resultCode, authActivityResult);

        switch (requestCode) {
            case AuthenticationActivity.AUTH_REQUEST_COMPLETE:
                if (resultCode != RESULT_OK) {
                    Toast.makeText(this, "Not able to sign in", Toast.LENGTH_SHORT).show();
                    finish();
                }
                break;
        }
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        //TODO: Save state to Bundle
    }

    private void restoreState(Bundle savedInstanceState) {
        //TODO: Restore state
    }


    private void emptyListOnUIThread() {
        runOnUiThread(new Runnable() {
            public void run() {
                ((EmployeeListFragment) getFragmentManager().findFragmentById(R.id.employee_list)).emptyEmployees();
            }
        });
    }

    private void updateListOnUIThread(final SearchResultModel data) {
        runOnUiThread(new Runnable() {
            public void run() {
                ((EmployeeListFragment) getFragmentManager().findFragmentById(R.id.employee_list)).updateEmployees(data);
            }
        });
    }

    @Override
    public boolean onQueryTextSubmit(String query) {
        if (!query.isEmpty()) {

            //TODO: Remove after proper implementation
            //showToastOnUiThread(this, searchString);

            final String token = new AuthenticationHelper().getJsonWebToken(this);

            //TODO: Get base from a more central place
            //TODO: Implement paging
            //String searchUrl = "http://milescontact.cloudapp.net/api/search/Fulltext?query=" + URLEncoder.encode(searchString);
            String searchUrl = "https://api-at.miles.no/api/search/Fulltext?query=" + URLEncoder.encode(query)+"&take=200";

            CallSearchApi callSearch = new CallSearchApi();
            callSearch.setProgressDialog(progress);

            //TODO: Save reference to be able to cancel if new search is started before the previous is completed
            callSearch.execute(searchUrl, token);
        } else {
            emptyListOnUIThread();
        }
        return false;
    }

    @Override
    public boolean onQueryTextChange(String newText) {

        return false;
    }


    //TODO: Move to service
    private class CallSearchApi extends AsyncTask<String, String, String> {

        private ProgressDialog bar;

        public void setProgressDialog(ProgressDialog bar){
            this.bar = bar;
        }

        @Override
        protected void onPreExecute() {
            bar.show();
        }

        @Override
        protected String doInBackground(String... params) {
            String urlString = params[0]; // URL to call
            String token = params[1]; // JWT

            String resultToDisplay = "";
            InputStream in = null;
            try {
                URL url = new URL(urlString);
                HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
                urlConnection.setRequestProperty("Authorization", "Bearer " + token);
                in = new BufferedInputStream(urlConnection.getInputStream());

                BufferedReader reader = new BufferedReader(new InputStreamReader(in));
                StringBuilder out = new StringBuilder();
                String line;
                while ((line = reader.readLine()) != null) {
                    out.append(line);
                }
                resultToDisplay = out.toString();
                reader.close();
            } catch (Exception e) {
                //TODO: Log better
                System.out.println(e.getMessage());
                return e.getMessage();
            }
            return resultToDisplay;
        }

        protected void onPostExecute(String result) {
            ObjectMapper mapper = new ObjectMapper();
            SearchResultModel resultObject = null;
            try {
                resultObject = mapper.readValue(result, SearchResultModel.class);
            } catch (IOException e) {
                //TODO: Log better
                e.printStackTrace();
            }
            updateListOnUIThread(resultObject);
            bar.dismiss();
        }
    }
}
