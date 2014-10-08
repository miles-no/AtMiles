package no.miles.atmiles;

import android.app.Activity;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;

import com.fasterxml.jackson.databind.ObjectMapper;

import org.apache.http.Header;
import org.apache.http.HeaderElement;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.ParseException;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;

import no.miles.atmiles.employee.SearchResultModel;


/**
 * An activity representing a list of Employees. This activity
 * has different presentations for handset and tablet-size devices. On
 * handsets, the activity presents a list of items, which when touched,
 * lead to a {@link EmployeeDetailActivity} representing
 * item details. On tablets, the activity presents the list of items and
 * item details side-by-side using two vertical panes.
 * <p>
 * The activity makes heavy use of fragments. The list of items is a
 * {@link EmployeeListFragment} and the item details
 * (if present) is a {@link EmployeeDetailFragment}.
 * <p>
 * This activity also implements the required
 * {@link EmployeeListFragment.Callbacks} interface
 * to listen for item selections.
 */
public class EmployeeListActivity extends Activity
        implements EmployeeListFragment.Callbacks, OnSearchStringChangeListener {


    /**
     * Whether or not the activity is in two-pane mode, i.e. running on a tablet
     * device.
     */
    private boolean mTwoPane;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_employee_list);

        if (findViewById(R.id.employee_detail_container) != null) {
            // The detail container view will be present only in the
            // large-screen layouts (res/values-large and
            // res/values-sw600dp). If this view is present, then the
            // activity should be in two-pane mode.
            mTwoPane = true;

            // In two-pane mode, list items should be given the
            // 'activated' state when touched.
            ((EmployeeListFragment) getFragmentManager()
                    .findFragmentById(R.id.employee_list))
                    .setActivateOnItemClick(true);
        }

        if(savedInstanceState != null) {
            restoreState(savedInstanceState);
        }

        // TODO: If exposing deep links into your app, handle intents here.
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
        return true;
    }

    /**
     * Callback method from {@link EmployeeListFragment.Callbacks}
     * indicating that the item with the given ID was selected.
     */
    @Override
    public void onItemSelected(String id) {
        if (mTwoPane) {
            // In two-pane mode, show the detail view in this activity by
            // adding or replacing the detail fragment using a
            // fragment transaction.
            Bundle arguments = new Bundle();
            arguments.putString(EmployeeDetailFragment.ARG_ITEM_ID, id);
            EmployeeDetailFragment fragment = new EmployeeDetailFragment();
            fragment.setArguments(arguments);
            getFragmentManager().beginTransaction()
                    .replace(R.id.employee_detail_container, fragment)
                    .commit();

        } else {
            // In single-pane mode, simply start the detail activity
            // for the selected item ID.
            Intent detailIntent = new Intent(this, EmployeeDetailActivity.class);
            detailIntent.putExtra(EmployeeDetailFragment.ARG_ITEM_ID, id);
            detailIntent.putExtra(EmployeeDetailFragment.ARG_ITEM_ID, id);
            startActivity(detailIntent);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        boolean handled = true;
        int id = item.getItemId();

        switch(id){
            case R.id.action_menu_favorites:
                startActivity(new Intent(this, FavoritesActivity.class));
                break;
            case R.id.action_menu_profile:
                startActivity(new Intent(this, ProfileActivity.class));
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent authActivityResult) {
        super.onActivityResult(requestCode, resultCode, authActivityResult);

        switch(requestCode)
        {
            case AuthenticationActivity.AUTH_REQUEST_COMPLETE:
                if(resultCode!=RESULT_OK)
                {
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

    @Override
    public void OnSearchStringChanged(String searchString) {
        if(!searchString.isEmpty()) {

            //TODO: Remove after proper implementation
            //showToastOnUiThread(this, searchString);

            final String token = new AuthenticationHelper().getJsonWebToken(this);

            //TODO: Get base from a more sentral place
            //TODO: Implement paging
            String searchUrl = "http://milescontact.cloudapp.net/api/search/Fulltext?query="+ URLEncoder.encode(searchString);


            //TODO: Save reference to be able to cancel if new search is started before the previous is completed
            new CallSearchApi().execute(searchUrl, token);
        }
        else{
            emptyListOnUIThread();
        }
    }

    private void emptyListOnUIThread()
    {
        runOnUiThread(new Runnable() {
            public void run() {
                ((EmployeeListFragment)getFragmentManager().findFragmentById(R.id.employee_list)).emptyEmployees();
            }
        });
    }

    private void updateListOnUIThread(final SearchResultModel data)
    {
        runOnUiThread(new Runnable() {
            public void run() {
                ((EmployeeListFragment)getFragmentManager().findFragmentById(R.id.employee_list)).updateEmployees(data);
            }
        });
    }

    private class CallSearchApi extends AsyncTask<String, String, String> {

        @Override
        protected String doInBackground(String... params) {
            String urlString=params[0]; // URL to call
            String token=params[1]; // JWT

            String resultToDisplay = "";
            InputStream in = null;

// HTTP Get
            try {
                URL url = new URL(urlString);
                HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
                urlConnection.setRequestProperty("Authorization", "Bearer "+token);
                in = new BufferedInputStream(urlConnection.getInputStream());

                BufferedReader reader = new BufferedReader(new InputStreamReader(in));
                StringBuilder out = new StringBuilder();
                String line;
                while ((line = reader.readLine()) != null) {
                    out.append(line);
                }
                resultToDisplay = out.toString();
                reader.close();
            } catch (Exception e ) {
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
        }
    }
}
